using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParseDirective : TestBase
    {
        private static readonly AppSettings DirectivesEnabled = TestAppSettings.TestDefault.Clone(s => s.EnableDirectives = true);

        public ParseDirective(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Should_EchoAllArgs_OnNewLine_WithIndent()
        {
            new AppRunner<App>(DirectivesEnabled)
                .UseParseDirective()
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[parse:verbose] some -ab args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @">>> from shell
  Directive: [parse:verbose]
  Value    : some
  Option   : -ab
  Value    : args
  Value    : to
  Value    : echo
>>> transformed after: expand-clubbed-flags
  Directive: [parse:verbose]
  Value    : some
  Option   : -a
  Option   : -b
  Value    : args
  Value    : to
  Value    : echo
>>> no changes after: split-option-assignments"
                        }
                    });
        }

        [Fact]
        public void Should_SpecifyWhenTransformation_DoesNotMakeChanges()
        {
            new AppRunner<App>(DirectivesEnabled)
                .UseParseDirective()
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[parse:verbose] some args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @">>> from shell
  Directive: [parse:verbose]
  Value    : some
  Value    : args
  Value    : to
  Value    : echo
>>> no changes after: expand-clubbed-flags
>>> no changes after: split-option-assignments"
                        }
                    });
        }

        public class App
        {
            public int some(
                [Option(ShortName = "a")] bool opt1,
                [Option(ShortName = "b")] bool opt2,
                [Option(ShortName = "val")] string optionValue,
                string args, string to, string echo)
            {
                return 5;
            }
        }
    }
}