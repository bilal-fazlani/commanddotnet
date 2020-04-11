using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BasicParseTests
    {
        private readonly ITestOutputHelper _output;

        public BasicParseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -o * 2 3",
                    Then = {Outputs = {new App.AddResults {X = 2, Y = 3, Op = "*"}}}
                });
        }

        [Fact]
        public void OptionCanBeSpecifiedAfterPositionalArg()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Add 2 3 -o *",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void OptionCanBeColonSeparated()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Add 2 3 -o:*",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void OptionCanBeEqualsSeparated()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Add 2 3 -o=*",
                Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "*" } } }
            });
        }

        [Fact]
        public void DoesNotModifySpecialCharactersInArguments()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgsArray = new[] { "Do", "~!@#$%^&*()_= +[]\\{} |;':\",./<>?" },
                Then =
                {
                    Outputs = { "~!@#$%^&*()_= +[]\\{} |;':\",./<>?" }
                }
            });
        }

        [Fact]
        public void BracketsShouldBeRetainedInText()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
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
            new AppRunner<App>().VerifyScenario(_output, new Scenario
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
            new AppRunner<App>().VerifyScenario(_output, new Scenario
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
        public void Given_IgnoreExtraOperands_DisabledByAppSetting_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add 2 3 4",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized command or argument '4'" }
                    }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByCommand_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>(new AppSettings { IgnoreUnexpectedOperands = true })
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_DisabledIgnore 2 3 4",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized command or argument '4'" }
                    }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByAppSettings_CollectsRemaining()
        {
            var results = new AppRunner<App>(new AppSettings { IgnoreUnexpectedOperands = true })
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add 2 3 4",
                    Then = { Outputs = { new App.AddResults { X = 2, Y = 3, Op = "+" } } }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEquivalentTo("4");
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByCommand_CollectsRemaining()
        {
            var results = new AppRunner<App>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_EnabledIgnore 2 3 4",
                    Then = { Outputs = { new App.AddResults { X = 2, Y = 3 } } }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEquivalentTo("4");
        }

        private class App
        {
            private TestOutputs TestOutputs { get; set; }

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

            [Command(IgnoreUnexpectedOperands = true)]
            public void Add_EnabledIgnore(int x, int y)
            {
                TestOutputs.Capture(new AddResults { X = x, Y = y });
            }

            [Command(IgnoreUnexpectedOperands = false)]
            public void Add_DisabledIgnore(int x, int y)
            {
                TestOutputs.Capture(new AddResults { X = x, Y = y });
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