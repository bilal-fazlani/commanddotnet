using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CommandRunnerTests : TestBase
    {
        public CommandRunnerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanInvokeCommand()
        {
            AppRunner<BasciApp> appRunner = new AppRunner<BasciApp>();
            string[] args = new[] {"Paint", "--color", "yellow"};
            int returnCode = appRunner.Run(args);
            returnCode.Should().Be(0);
        }
    }
    
    public class BasciApp
    {
        public BasciApp(int value)
        {
            
        }
        
        public void Paint(string color)
        {
            
        }
    }
}