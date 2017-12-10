using System;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class FlagTests
    {
        [Theory]
        [InlineData(new object[] {new[] {"CommandWithFlagTrue"}, 0})]
        [InlineData(new object[] {new[] {"CommandWithFlagFalse"}, 0})]
        [InlineData(new object[] {new[] {"CommandWithoutFlag"}, 0})]
        public void TestValidFlags(string[] inputArguments, int expectedExitCode)
        {
            AppRunner<ValidFlagsApplication> appRunner = new AppRunner<ValidFlagsApplication>();
            int exitCode = appRunner.Run(inputArguments);
            exitCode.Should().Be(expectedExitCode);
        }

        [Theory]
        [InlineData(new object[] {new[] {"CommandWithInvalidFlag"}, 1})]
        public void TestInvalidFlags(string[] inputArguments, int expectedExitCode)
        {
            AppRunner<ValidFlagsApplication> appRunner = new AppRunner<ValidFlagsApplication>();
            int exitCode = appRunner.Run(inputArguments);
            exitCode.Should().Be(expectedExitCode);
        }
    }

    public class ValidFlagsApplication
    {
        public void CommandWithFlagTrue([Argument(Flag = true)] bool flag)
        {
        }

        public void CommandWithFlagFalse([Argument(Flag = false)] bool flag)
        {
        }

        public void CommandWithoutFlag(bool flag)
        {
        }
    }

    public class InvalidFlagApplication
    {
        public void CommandWithInvalidFlag([Argument(Flag = true)] string flag)
        {
        }
    }
}