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
                        WhenArgs = "[parse:transforms] some -ab args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @"command: some

arguments:

  args <Text>
    value: args
    inputs: args

  to <Text>
    value: to
    inputs: to

  echo <Text>
    value: echo
    inputs: echo

options:

  a <Flag>
    value: True
    inputs: true (from: -ab -> -a)

  b <Flag>
    value: True
    inputs: true (from: -ab -> -b)

  v <Text>
    value:


token transformations:

>>> from shell
  Directive: [parse:transforms]
  Value    : some
  Option   : -ab
  Value    : args
  Value    : to
  Value    : echo
>>> transformed after: expand-clubbed-flags
  Directive: [parse:transforms]
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
                        WhenArgs = "[parse:transforms] some args to echo",
                        Then =
                        {
                            ExitCode = 0, // method should not have been called
                            Result = @"command: some

arguments:

  args <Text>
    value: args
    inputs: args

  to <Text>
    value: to
    inputs: to

  echo <Text>
    value: echo
    inputs: echo

options:

  a <Flag>
    value:

  b <Flag>
    value:

  v <Text>
    value:


token transformations:

>>> from shell
  Directive: [parse:transforms]
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