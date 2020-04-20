using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BasicParseTests
    {
        public BasicParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add -o * 2 3"},
                Then = {Captured = {new App.AddResults {X = 2, Y = 3, Op = "*"}}}
            });
        }

        [Fact]
        public void OptionCanBeSpecifiedAfterPositionalArg()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o *"},
                Then = {Captured = {new App.AddResults {X = 2, Y = 3, Op = "*"}}}
            });
        }

        [Fact]
        public void OptionCanBeColonSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o:*"},
                Then = {Captured = {new App.AddResults {X = 2, Y = 3, Op = "*"}}}
            });
        }

        [Fact]
        public void OptionCanBeEqualsSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o=*"},
                Then = {Captured = {new App.AddResults {X = 2, Y = 3, Op = "*"}}}
            });
        }

        [Fact]
        public void DoesNotModifySpecialCharactersInArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "~!@#$%^&*()_= +[]\\{} |;':\",./<>?"}},
                Then = {Captured = {"~!@#$%^&*()_= +[]\\{} |;':\",./<>?"}}
            });
        }

        [Fact]
        public void BracketsShouldBeRetainedInText()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "[some (parenthesis) {curly} and [bracketed] text]"}},
                Then = {Captured = {"[some (parenthesis) {curly} and [bracketed] text]"}}
            });
        }

        [Fact(Skip = "Method params cannot be marked as required yet.  Requiredness is only possible via FluentValidator")]
        public void OperandsAreRequired()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"missing argument 'Y'"}
                }
            });
        }

        [Fact]
        public void Given_OptionRequiresValue_When_NextTokenIsOption_ThrowsError()
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
        public void ErrorWhenExtraValueProvidedForOption()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o * %"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"Unrecognized command or argument '%'"}
                }
            });
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByAppSetting_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Add 2 3 4"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByCommand_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>(new AppSettings {IgnoreUnexpectedOperands = true})
                .Verify(new Scenario
                {
                    When = {Args = "Add_DisabledIgnore 2 3 4"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByAppSettings_CollectsRemaining()
        {
            var results = new AppRunner<App>(new AppSettings {IgnoreUnexpectedOperands = true})
                .Verify(new Scenario
                {
                    When = {Args = "Add 2 3 4"},
                    Then = {Captured = {new App.AddResults {X = 2, Y = 3, Op = "+"}}}
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEquivalentTo("4");
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByCommand_CollectsRemaining()
        {
            var results = new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Add_EnabledIgnore 2 3 4"},
                    Then = {Captured = {new App.AddResults {X = 2, Y = 3}}}
                });

            results.CommandContext.ParseResult.RemainingOperands.Should().BeEquivalentTo("4");
        }

        private class App
        {
            private TestCaptures TestCaptures { get; set; }

            public void Add(
                [Operand(Description = "the first operand")] int x,
                [Operand(Description = "the second operand")] int y,
                [Option(ShortName = "o", LongName = "operator", Description = "the operation to apply")]
                string operation = "+")
            {
                TestCaptures.Capture(new AddResults { X = x, Y = y, Op = operation });
            }

            [Command(IgnoreUnexpectedOperands = true)]
            public void Add_EnabledIgnore(int x, int y)
            {
                TestCaptures.Capture(new AddResults { X = x, Y = y });
            }

            [Command(IgnoreUnexpectedOperands = false)]
            public void Add_DisabledIgnore(int x, int y)
            {
                TestCaptures.Capture(new AddResults { X = x, Y = y });
            }

            public void Do([Operand] string arg)
            {
                TestCaptures.Capture(arg);
            }

            public void Options([Option] string opt1, [Option] string opt2)
            {
                TestCaptures.Capture(new[] {opt1, opt2});
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