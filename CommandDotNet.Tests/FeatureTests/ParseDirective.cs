using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParseDirective : TestBase
    {
        public ParseDirective(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Should_EchoAllArgs_OnNewLine_WithIndent()
        {
            Verify(new Given<App>
            {
                WhenArgs = "[parse] some args to echo",
                Then =
                {
                    ExitCode = 0, // method should not have been called
                    Result = @"some
args
to
echo"
                }
            });
        }

        public class App
        {
            public int Do()
            {
                return 5;
            }
        }
    }
}