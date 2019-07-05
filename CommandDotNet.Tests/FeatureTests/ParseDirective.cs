using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParseDirective : TestBase
    {
        private readonly AppSettings DirectivesEnabled = TestAppSettings.TestDefault.Clone(s => s.EnableDirectives = true);

        public ParseDirective(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Should_EchoAllArgs_OnNewLine_WithIndent()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DirectivesEnabled },
                WhenArgs = "[parse] some -ab args to echo",
                Then =
                {
                    ExitCode = 0, // method should not have been called
                    Result = @">>> from shell
  Value    : some
  Option   : -ab
  Value    : args
  Value    : to
  Value    : echo
>>> transformed after: Expand clubbed flags
  Value    : some
  Option   : -a
  Option   : -b
  Value    : args
  Value    : to
  Value    : echo"
                }
            });
        }

        [Fact]
        public void Should_SpecifyWhenTransformation_DoesNotMakeChanges()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DirectivesEnabled },
                WhenArgs = "[parse] some args to echo",
                Then =
                {
                    ExitCode = 0, // method should not have been called
                    Result = @">>> from shell
  Value    : some
  Value    : args
  Value    : to
  Value    : echo
>>> no changes after: Expand clubbed flags"
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