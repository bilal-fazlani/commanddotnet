using System;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandRunnerTests
    {
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
        public void Paint(string color)
        {
            
        }
    }
}