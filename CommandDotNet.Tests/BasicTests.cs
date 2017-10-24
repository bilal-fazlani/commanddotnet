using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test1()
        {
            CommandHelper<MyCommands> commandHelper = new CommandHelper<MyCommands>();
            string[] args = new[] {"Paint", "--color", "yellow"};
            int returnCode = commandHelper.Run(args);
            returnCode.Should().Be(0);
        }
    }
}