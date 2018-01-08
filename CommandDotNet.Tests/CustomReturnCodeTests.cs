using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CustomReturnCodeTests : TestBase
    {
        public CustomReturnCodeTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("VoidMethodThatHasNoException", 0)]
        [InlineData("IntMethodWithNoException", 4)]
        public void Test(string commandName, int expectedExitCode)
        {
            AppRunner<AppForTestingReturnCodes> appRunner = new AppRunner<AppForTestingReturnCodes>();
            int actualExitCode = appRunner.Run(new[] {commandName});
            actualExitCode.Should().Be(expectedExitCode);
        }
    }

    public class AppForTestingReturnCodes
    {
        public void VoidMethodThatHasNoException()
        {
            
        }

        public int IntMethodWithNoException()
        {
            return 4;
        }
    }
}