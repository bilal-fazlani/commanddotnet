using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CustomReturnCodeTests
    {
        [Theory]
        [InlineData("VoidMethodThatHasNoException", 0)]
        [InlineData("VoidMethodWithException", 1)]
        [InlineData("IntMethodWithNoException", 4)]
        [InlineData("IntMethodWithException", 1)]
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

        public void VoidMethodWithException()
        {
            throw new Exception();
        }

        public int IntMethodWithNoException()
        {
            return 4;
        }
        
        public int IntMethodWithException()
        {
            throw new Exception();
        }
    }
}