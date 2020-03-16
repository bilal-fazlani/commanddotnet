using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParseDirectiveTests : TestBase
    {
        public ParseDirectiveTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Should_EchoAllArgs_OnNewLine_WithIndent()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[parse:tokens;verbose] some -ab args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @"use [parse:help] to see additional parse options
>>> from shell
  Directive: [parse:tokens;verbose]
  Value    : some
  Option   : -ab
  Value    : args
  Value    : to
  Value    : echo
>>> transformed after: expand-clubbed-flags
  Directive: [parse:tokens;verbose]
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
            new AppRunner<App>()
                .UseParseDirective()
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[parse:tokens;verbose] some args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @"use [parse:help] to see additional parse options
>>> from shell
  Directive: [parse:tokens;verbose]
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
                [Option(ShortName = "v")] string optionValue,
                string args, string to, string echo)
            {
                return 5;
            }
        }
    }
}