using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BasicParseScenarios : TestBase
    {
        public BasicParseScenarios(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add -o * 2 3",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void OptionCanBeSpecifiedAfterPositionalArg()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2 3 -o *",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void OptionCanBeColonSeparated()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2 3 -o:*",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void OptionCanBeEqualsSeparated()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2 3 -o=*",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void DoesNotModifySpecialCharactersInArguments()
        {
            Verify(new Scenario<App>("exec - special characters should be retained")
            {
                WhenArgsArray = new[] { "Do", "~!@#$%^&*()_= +[]\\{} |;':\",./<>?" },
                Then =
                {
                    Outputs = { "~!@#$%^&*()_= +[]\\{} |;':\",./<>?" }
                }
            });
        }

        [Fact]
        public void BracketsShouldbeRetainedInText()
        {
            Verify(new Scenario<App>
            {
                WhenArgsArray = new[] { "Do", "[some (parenthesis) {curly} and [bracketed] text]" },
                Then =
                {
                    Outputs = { "[some (parenthesis) {curly} and [bracketed] text]" }
                }
            });
        }

        [Fact(Skip = "Method params cannot be marked as required yet.  Requiredness is only possible via FluentValidator")]
        public void OperandsAreRequired()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = {"missing argument 'Y'"}
                }
            });
        }

        [Fact]
        public void ErrorWhenExtraValueProvidedForOption()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2 3 -o * %",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = {"Unrecognized command or argument '%'"}
                }
            });
        }

        [Fact]
        public void ExtraArgumentsNotAllowed()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Add 2 3 4",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized command or argument '4'" }
                }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Add(
                [Operand(Description = "the first operand")]
                int x,
                [Operand(Description = "the second operand")]
                int y,
                [Option(ShortName = "o", LongName = "operator", Description = "the operation to apply")]
                string operation = "+")
            {
                TestOutputs.Capture(new AddResults { X = x, Y = y, Op = operation });
            }

            public void Do([Operand] string arg)
            {
                TestOutputs.Capture(arg);
            }

            public class AddResults
            {
                public int X { get; set; }
                public int Y { get; set; }
                public string Op { get; set; }
            }
        }
    }
}