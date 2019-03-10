using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CaseTests : TestBase
    {
        public CaseTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(Case.DontChange, "ProcessRequest")]
        [InlineData(Case.CamelCase, "processRequest")]
        [InlineData(Case.PascalCase, "ProcessRequest")]
        [InlineData(Case.KebabCase, "process-request")]
        [InlineData(Case.LowerCase, "processrequest")]
        [InlineData(Case.DontChange, "wn")]
        [InlineData(Case.CamelCase, "wn")]
        [InlineData(Case.PascalCase, "wn")]
        [InlineData(Case.KebabCase, "wn")]
        [InlineData(Case.LowerCase, "wn")]
        public void CanHonorCommandCase(Case @case, string commandName)
        {
            AppRunner<CommandCaseApp> appRunner = new AppRunner<CommandCaseApp>(new AppSettings
            {
                Case = @case
            });
            appRunner.Run(commandName).Should().Be(10);
        }
        
        
        [Theory]
        [InlineData(Case.DontChange, "Send", "--messageName", "--Sender", "-P", "-c")]
        [InlineData(Case.CamelCase, "send", "--messageName", "--Sender", "-P", "-c")]
        [InlineData(Case.PascalCase, "Send", "--MessageName", "--Sender", "-P", "-C")]
        [InlineData(Case.KebabCase, "send", "--message-name", "--Sender", "-P", "-c")]
        [InlineData(Case.LowerCase, "send", "--messagename", "--Sender", "-P", "-c")]
        public void CanHonorOptionCase(Case @case, string commandName, string messageName, 
            string senderName, string priorotyName, string cName)
        {
            AppRunner<CommandCaseApp> appRunner = new AppRunner<CommandCaseApp>(new AppSettings
            {
                Case = @case
            });
            appRunner.Run(commandName, messageName, "m", senderName, "s", priorotyName, "3", cName, "4" ).Should().Be(10);
        }

        [Theory]
        [InlineData(Case.DontChange, "SubCommand", "SendNotification")]
        [InlineData(Case.CamelCase, "subCommand", "sendNotification")]
        [InlineData(Case.PascalCase,"SubCommand", "SendNotification")]
        [InlineData(Case.KebabCase, "sub-command", "send-notification")]
        [InlineData(Case.LowerCase, "subcommand", "sendnotification")]
        public void CanHonorSubCommandCase(Case @case, string commandName, string notificationCommandName)
        {
            AppRunner<CommandCaseApp> appRunner = new AppRunner<CommandCaseApp>(new AppSettings
            {
                Case = @case
            });
            
            appRunner.Run(commandName, notificationCommandName).Should().Be(10);
        }
    }
    
    public class CommandCaseApp
    {
        public int ProcessRequest()
        {
            return 10;
        }

        public int Send([Option] string messageName, [Option(LongName = "Sender")] string senderName, 
            [Option(ShortName = "P")] int priority, [Option]int c)
        {
            return messageName == "m" && senderName == "s" && priority == 3 && c == 4 ? 10 : 1;
        }
        
        [ApplicationMetadata(Name = "wn")]
        public int WithName()
        {
            return 10;
        }

        [SubCommand]
        public class SubCommand
        {
            public int SendNotification()
            {
                return 10;
            }
        }
    }
}