using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help;

public class GeneralHelpTests
{
    public GeneralHelpTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void QuestionMark_ShowsHelp()
    {
        var result = new AppRunner<App>().RunInMem("-?".SplitArgs());
        result.CommandContext.ShowHelpOnExit.Should().BeTrue();
    }

    [Fact]
    public void ShortName_ShowsHelp()
    {
        var result = new AppRunner<App>().RunInMem("-h".SplitArgs());
        result.CommandContext.ShowHelpOnExit.Should().BeTrue();
    }

    [Fact]
    public void LongName_ShowsHelp()
    {
        var result = new AppRunner<App>().RunInMem("--help".SplitArgs());
        result.CommandContext.ShowHelpOnExit.Should().BeTrue();
    }

    [Fact]
    public void Help_localizes_command_and_arg_help_texts()
    {
        new AppRunner<App>(new AppSettings {Localization = { Localize = loc }})
            .AfterRun(r => Resources.A = new Resources())
            .Verify(new Scenario
            {
                When = { Args = "Do --help" },
                Then = { Output = @"aaa

Usage: bbb

Arguments:

  opn1  <TEXT>
  ccc

Options:

  --opt1  <TEXT>
  ddd

eee" }
            });
        return;

        static string loc(string arg)
        {
            return arg switch
            {
                "cmd-desc" => "aaa",
                "cmd-usg" => "bbb",
                "opn1-desc" => "ccc",
                "opt1-desc" => "ddd",
                "cmd-eht" => "eee",
                _ => arg
            };
        }
    }

    [UsedImplicitly]
    private class App
    {
        [Command(Description = "cmd-desc", Usage = "cmd-usg", ExtendedHelpText = "cmd-eht")]
        public void Do(
            [Option(Description = "opt1-desc")] string opt1,
            [Operand(Description = "opn1-desc")] string opn1)
        {
        }
    }
}