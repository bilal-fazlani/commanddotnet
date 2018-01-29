using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class EnumTests : TestBase
    {
        public EnumTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("Now", 3)]
        [InlineData("Yesterday", 4)]
        [InlineData("Tomorrow", 5)]
        [InlineData("Invalid", 2)]
        public void CanParseEnums(string input,int expectedExitCode)
        {
            AppRunner<EnumApp> appRunner = new AppRunner<EnumApp>();
            appRunner.Run("parseEnum", input).Should().Be(expectedExitCode, $"'{input} = {expectedExitCode}'");
        }

        [Fact]
        public void CanHandleNoValueForEnums()
        {
            AppRunner<EnumApp> appRunner = new AppRunner<EnumApp>();
            appRunner.Run("parseEnum").Should().Be(0);
        }

        [Fact]
        public void CanParseListOfEnum()
        {
            AppRunner<EnumApp> appRunner = new AppRunner<EnumApp>();
            appRunner.Run("ParseAll", "Yesterday", "Tomorrow").Should().Be(2);
        }
    }

    public class EnumApp
    {
        public int ParseEnum(Time time)
        {
            return (int) time;
        }

        public int ParseAll(List<Time> times)
        {
            times[0].Should().Be(Time.Yesterday);
            times[1].Should().Be(Time.Tomorrow);
            
            return times.Count;
        }
    }

    public enum Time
    {
        Now = 3,
        Yesterday = 4,
        Tomorrow = 5
    }
}