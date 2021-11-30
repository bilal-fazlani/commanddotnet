using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests
{
    public class OptionArityParseTests
    {
        public OptionArityParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void GivenOptionExpectingAValue_ValueMustBeSpecified_BeforeEndOfLine()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Options --opt1" },
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"Missing value for option 'opt1'"}
                }
            });
        }

        [Fact(Skip = "--opt2 will be treated as a value for --opt1 now")]
        // TODO: identify when --opt2 could be an option and --opt1 is missing value
        public void GivenOptionExpectingAValue_ValueMustBeSpecified_BeforeNextOption()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Options --opt1 --opt2 go" },
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"Missing value for option 'opt1'"}
                }
            });
        }

        [Fact]
        public void GivenOptionExpectingAValue_Subcommands_WillBeUsedAsOptionValue()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Options --opt1 Subby" },
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe("Subby", null)
                }
            });
        }

        private class App
        {
            public void Options([Option] string? opt1, [Option] string? opt2)
            {
            }

            [Subcommand]
            public class Subby
            {

            }
        }
    }
}