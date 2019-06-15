using CommandDotNet.Attributes;
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
                WhenArgs = "[parse] some -ab args to echo",
                Then =
                {
                    ExitCode = 0, // method should not have been called
                    Result = @"received:
   some
   -ab
   args
   to
   echo
transformation: unclub-flags
   some
   -a
   -b
   args
   to
   echo"
                }
            });
        }

        [Fact]
        public void Should_SpecifyWhenTransformation_DoesNotMakeChanges()
        {
            Verify(new Given<App>
            {
                WhenArgs = "[parse] some args to echo",
                Then =
                {
                    ExitCode = 0, // method should not have been called
                    Result = @"received:
   some
   args
   to
   echo
transformation: unclub-flags (no changes)"
                }
            });
        }

        public class App
        {
            public int some(
                [Option(ShortName = "a")] bool opt1,
                [Option(ShortName = "b")] bool opt2,
                string args, string to, string echo)
            {
                return 5;
            }
        }
    }
}