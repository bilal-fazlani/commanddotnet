using System;
using System.Collections.Generic;
using System.CommandLine.Suggest;
using System.IO;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.DotnetSuggest;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.SuggestDirective;

public class SuggestDirectiveRegistrationTests
{
    private readonly ITestOutputHelper _output;
    private readonly string _filePath = Path.Join(nameof(SuggestDirectiveRegistrationTests), "suggest-test.exe");

    public SuggestDirectiveRegistrationTests(ITestOutputHelper output)
    {
        _output = output;
        Ambient.Output = output;
    }

    [Theory]
    [InlineData("[suggest]", RegistrationStrategy.None, null, false, false)]
    [InlineData("", RegistrationStrategy.None, null, false, false)]
    [InlineData("[suggest]", RegistrationStrategy.EnsureOnEveryRun, null, false, false)]
    [InlineData("", RegistrationStrategy.EnsureOnEveryRun, null, false, true)]
    [InlineData("[suggest]", RegistrationStrategy.UseRegistrationDirective, "sug", false, false)]
    [InlineData("", RegistrationStrategy.UseRegistrationDirective, "sug", false, false)]
    [InlineData("[sug]", RegistrationStrategy.UseRegistrationDirective, "sug", false, true)]
    [InlineData("[sug]", RegistrationStrategy.None, "sug", false, false)]
    [InlineData("", RegistrationStrategy.EnsureOnEveryRun, null, true, false)]
    [InlineData("[sug]", RegistrationStrategy.UseRegistrationDirective, "sug", true, false)]
    public void Suggest_can_register_with_Dotnet_Suggest(string args, RegistrationStrategy strategy, string? directive, bool isGlobal, bool shouldRegister)
    {
        #region ensure test cases are not misconfigured and summarize a few of the rules
        if (strategy == RegistrationStrategy.None)
        {
            shouldRegister.Should().Be(false, "should never register unless ensureRegisteredWithDotnetSuggest=true");
        }
        if(args.StartsWith("[suggest]"))
        {
            shouldRegister.Should().Be(false, "should never register when providing suggestions");
        }
        if(isGlobal)
        {
            shouldRegister.Should().Be(false, "should never register when app is global tool");
        }
        #endregion 
        
        var result = new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseCommandLogger()
            .UseSuggestDirective_Experimental(strategy, directive!)
            .UseTestEnv(new ())
            .Verify(new Scenario
                {
                    When = {Args = args},
                    Then = { Output = args.StartsWith("[sug") ? null : "lala" }
                },
                config: TestConfig.Default.Where(a => a.AppInfoOverride = BuildAppInfo(isGlobal)));

        result.ExitCode.Should().Be(0);

        ConfirmPathEnvVar(shouldRegister, result);
        
        ConfirmRegistration(shouldRegister);
    }

    private void ConfirmPathEnvVar(bool shouldRegister, AppRunnerResult result)
    {
        var testEnvironment = (TestEnvironment) result.CommandContext.Environment;
        var userEnvVars = testEnvironment.EnvVarByTarget.GetValueOrDefault(EnvironmentVariableTarget.User);
        if (shouldRegister)
        {
            userEnvVars.Should().NotBeNull();
            var pathEnvVar = userEnvVars!.GetValueOrDefault("PATH");
            pathEnvVar.Should().NotBeNull();
            pathEnvVar.Should().Contain(_filePath);
        }
        else
        {
            userEnvVars?.GetValueOrDefault("PATH")?.Should().NotContain(_filePath);
        }
    }

    private static void ConfirmRegistration(bool shouldRegister)
    {
        var path = new FileSuggestionRegistration().RegistrationConfigurationFilePath;
        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);

            // _output.WriteLine($"contents of {path}");
            // lines.ForEach(l => _output.WriteLine(l));
            // 
            // contents of /Users/{user}/.dotnet-suggest-registration.txt
            // SuggestDirectiveRegistrationTests/suggest-test.exe

            var cleanedLines = lines.Where(l => !l.StartsWith(nameof(SuggestDirectiveRegistrationTests))).ToArray();
            File.WriteAllLines(path, cleanedLines);

            if (shouldRegister)
            {
                lines.Length.Should().NotBe(cleanedLines.Length);
            }
            else
            {
                lines.Length.Should().Be(cleanedLines.Length);
            }
        }
    }

    private AppInfo BuildAppInfo(bool isGlobalTool)
    {
        return new(
            false, false, false, isGlobalTool, GetType().Assembly,
            _filePath, Path.GetFileName(_filePath));
    }

    public class App
    {
        [DefaultCommand]
        public void Do(IConsole console) => console.WriteLine("lala");
    }
}