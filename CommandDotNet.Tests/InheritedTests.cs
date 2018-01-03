using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class InheritedTests : TestBase
    {
        public InheritedTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanInheriOptions()
        {
            AppRunner<InheritedApp> appRunner = new AppRunner<InheritedApp>();
            appRunner.Run("--value", "10", "GetValue").Should().Be(10, "10 is passed as root option");
            appRunner.Run("GetValue", "--value", "10").Should().Be(10, "10 is passed as command option");
            appRunner.Run("GetValue").Should().Be(0, "no option is passed");
        }
    }

    public class InheritedApp
    {
        private readonly int _value;

        public InheritedApp([Option(Inherited = true)]int value)
        {
            _value = value;
        }

        public int GetValue()
        {
            return _value;
        }
    }
}