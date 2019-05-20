using CommandDotNet.Tests.SmokeTests.Apps;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.SmokeTests
{
    public class SingleMethodAppTests
    {
        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            var results = new AppRunner<SingleMethodApp>().RunInMem("Add", "2", "3");
            results.ExitCode.Should().Be(0);
            
            results.ConsoleOut.Should().Be("2+3=5");
            
            var inputs = results.TestOutputs.Get<SingleMethodApp>();
            inputs.X.Should().Be(2);
            inputs.Y.Should().Be(3);
        }
    }
}