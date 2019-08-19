using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomReturnCodeTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CustomReturnCodeTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("VoidMethodThatHasNoException", 0)]
        [InlineData("IntMethodWithNoException", 4)]
        public void Test(string commandName, int expectedExitCode)
        {
            var result = new AppRunner<App>()
                .RunInMem(new[] { commandName }, _testOutputHelper);
            
            result.ExitCode.Should().Be(expectedExitCode);
        }

        public class App
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
}