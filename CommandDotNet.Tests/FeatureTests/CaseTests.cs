using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CaseTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CaseTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
            var result = new AppRunner<App>(new AppSettings {Case = @case})
                .RunInMem(new[] {commandName}, _testOutputHelper);

            result.ExitCode.Should().Be(10);
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
            var args = new[] { commandName, messageName, "m", senderName, "s", priorotyName, "3", cName, "4" };
            var result = new AppRunner<App>(new AppSettings { Case = @case })
                .RunInMem(args, _testOutputHelper);
            
            result.ExitCode.Should().Be(10);
        }

        [Theory]
        [InlineData(Case.DontChange, "SubCommand", "SendNotification")]
        [InlineData(Case.CamelCase, "subCommand", "sendNotification")]
        [InlineData(Case.PascalCase,"SubCommand", "SendNotification")]
        [InlineData(Case.KebabCase, "sub-command", "send-notification")]
        [InlineData(Case.LowerCase, "subcommand", "sendnotification")]
        public void CanHonorSubCommandCase(Case @case, string commandName, string notificationCommandName)
        {
            var result = new AppRunner<App>(new AppSettings { Case = @case })
                .RunInMem(new[] { commandName, notificationCommandName }, _testOutputHelper);
            
            result.ExitCode.Should().Be(10);
        }

        public class App
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

            [Command(Name = "wn")]
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
}