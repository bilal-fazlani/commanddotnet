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
        [InlineData("Invalid", 1)]
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
    }

    public class EnumApp
    {
        public int ParseEnum(Time time)
        {
            return (int) time;
        }
    }

    public enum Time
    {
        Now = 3,
        Yesterday = 4,
        Tomorrow = 5
    }
}