using System.Linq;
using CommandDotNet.NameCasing;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.NameCasing;

public class CaseTests
{
    public CaseTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Theory]
    [InlineData(Case.DontChange, "ProcessRequest")]
    [InlineData(Case.CamelCase, "processRequest")]
    [InlineData(Case.PascalCase, "ProcessRequest")]
    [InlineData(Case.KebabCase, "process-request")]
    [InlineData(Case.SnakeCase, "process_request")]
    [InlineData(Case.LowerCase, "processrequest")]
    [InlineData(Case.DontChange, "wn")]
    [InlineData(Case.CamelCase, "wn")]
    [InlineData(Case.PascalCase, "wn")]
    [InlineData(Case.KebabCase, "wn")]
    [InlineData(Case.SnakeCase, "wn")]
    [InlineData(Case.LowerCase, "wn")]
    public void CanHonorCommandCase(Case @case, string commandName)
    {
        var result = new AppRunner<App>()
            .UseNameCasing(@case)
            .RunInMem([commandName]);

        result.ExitCode.Should().Be(10);
    }
        
    [Theory]
    [InlineData(Case.DontChange, "Send", "--messageName", "--Sender", "-P", "-c")]
    [InlineData(Case.CamelCase, "send", "--messageName", "--Sender", "-P", "-c")]
    [InlineData(Case.PascalCase, "Send", "--MessageName", "--Sender", "-P", "-C")]
    [InlineData(Case.KebabCase, "send", "--message-name", "--Sender", "-P", "-c")]
    [InlineData(Case.SnakeCase, "send", "--message_name", "--Sender", "-P", "-c")]
    [InlineData(Case.LowerCase, "send", "--messagename", "--Sender", "-P", "-c")]
    public void CanHonorOptionCase(Case @case, string commandName, string messageName, 
        string senderName, string priorotyName, string cName)
    {
        var args = new[] { commandName, messageName, "m", senderName, "s", priorotyName, "3", cName, "4" };
        var result = new AppRunner<App>()
            .UseNameCasing(@case)
            .RunInMem(args);
            
        result.ExitCode.Should().Be(10);
    }

    [Theory]
    [InlineData(Case.DontChange, "SubCommand", "SendNotification")]
    [InlineData(Case.CamelCase, "subCommand", "sendNotification")]
    [InlineData(Case.PascalCase,"SubCommand", "SendNotification")]
    [InlineData(Case.KebabCase, "sub-command", "send-notification")]
    [InlineData(Case.SnakeCase, "sub_command", "send_notification")]
    [InlineData(Case.LowerCase, "subcommand", "sendnotification")]
    public void CanHonorSubCommandCase(Case @case, string commandName, string notificationCommandName)
    {
        var result = new AppRunner<App>()
            .UseNameCasing(@case)
            .RunInMem([commandName, notificationCommandName]);
            
        result.ExitCode.Should().Be(10);
    }

    [Theory]
    [InlineData(Case.DontChange, "RenamedCommand", "SendNotification")]
    [InlineData(Case.CamelCase, "renamedCommand", "sendNotification")]
    [InlineData(Case.PascalCase, "RenamedCommand", "SendNotification")]
    [InlineData(Case.KebabCase, "renamed-command", "send-notification")]
    [InlineData(Case.SnakeCase, "renamed_command", "send_notification")]
    [InlineData(Case.LowerCase, "renamedcommand", "sendnotification")]
    public void CanHonorRenamedSubCommandCase(Case @case, string commandName, string notificationCommandName)
    {
        var result = new AppRunner<App>()
            .UseNameCasing(@case, applyToNameOverrides: true)
            .RunInMem([commandName, notificationCommandName]);

        result.ExitCode.Should().Be(10);
    }

    [Theory]
    [InlineData(Case.DontChange, false, "camelCase", "kebab-case", "snake_case", "lowercase", "NoOverride", "PascalCase")]
    [InlineData(Case.CamelCase, false, "camelCase", "kebab-case", "snake_case", "lowercase", "noOverride", "PascalCase")]
    [InlineData(Case.KebabCase, false, "camelCase", "kebab-case", "snake_case", "lowercase", "no-override", "PascalCase")]
    [InlineData(Case.SnakeCase, false, "camelCase", "kebab-case", "snake_case", "lowercase", "no_override", "PascalCase")]
    [InlineData(Case.LowerCase, false, "camelCase", "kebab-case", "snake_case", "lowercase", "nooverride", "PascalCase")]
    [InlineData(Case.PascalCase, false, "camelCase", "kebab-case", "snake_case", "lowercase", "NoOverride", "PascalCase")]
    [InlineData(Case.DontChange, true, "camelCase", "kebab-case", "snake_case", "lowercase", "NoOverride", "PascalCase")]
    [InlineData(Case.CamelCase, true, "camelCase", "kebab-case", "snakeCase", "lowercase", "noOverride", "pascalCase")]
    [InlineData(Case.KebabCase, true, "camel-case", "kebab-case", "snake-case", "lowercase", "no-override", "pascal-case")]
    [InlineData(Case.SnakeCase, true, "camel_case", "kebab_case", "snake_case", "lowercase", "no_override", "pascal_case")]
    [InlineData(Case.LowerCase, true, "camelcase", "kebab-case", "snake_case", "lowercase", "nooverride", "pascalcase")]
    [InlineData(Case.PascalCase, true, "CamelCase", "KebabCase", "SnakeCase", "Lowercase", "NoOverride", "PascalCase")]
    public void CanChangeCaseOfOverride(Case @case, bool applyToNameOverrides, params string[] commandNames)
    {
        // Important things to know
        // - lowercase cannot be converted to another case... except, the first letter will be capitalized for Pascal
        //   Humanizer doesn't know where the second word starts
        // - kebabcase cannot be converted to camelcase or lowercase. no idea why atm.
        // - Camel and Pascal can be converted to any other case

        new AppRunner<App2>()
            .UseNameCasing(@case, applyToNameOverrides)
            .Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    OutputContainsTexts = commandNames.ToList()
                }
            });
    }

    class App2
    {
        public void NoOverride() { }

        [Command("camelCase")]
        public void CamelCaseOverride() { }

        [Command("kebab-case")]
        public void KebabCaseOverride() { }

        [Command("snake_case")]
        public void SnakeCaseOverride() { }

        [Command("lowercase")]
        public void LowerCaseOverride() { }

        [Command("PascalCase")]
        public void PascalCaseOverride() { }
    }

    class App
    {
        public int ProcessRequest() => 10;

        public int Send([Option] string messageName, [Option("Sender")] string senderName,
            [Option('P')] int priority, [Option]int c) =>
            messageName == "m" && senderName == "s" && priority == 3 && c == 4 ? 10 : 1;

        [Command("wn")]
        public int WithName()
        {
            return 10;
        }

        [Subcommand]
        public class SubCommand
        {
            public int SendNotification() => 10;
        }

        [Subcommand(RenameAs = "RenamedCommand")]
        public class OtherSubCommand
        {
            public int SendNotification() => 10;
        }
    }
}