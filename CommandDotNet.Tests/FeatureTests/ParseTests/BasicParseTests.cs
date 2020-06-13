using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests
{
    public class BasicParseTests
    {
        public BasicParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void OptionCanBeSpecifiedBeforePositionalArg()
        {
            var result = new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add -o * 2 3"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*", 10) }
            });
        }

        [Fact]
        public void OptionCanBeSpecifiedAfterPositionalArg()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o *"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*", 10) }
            });
        }

        [Fact]
        public void OptionCanBeSpecifiedBetweenPositionalArgs()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Add 2 -o * 3" },
                Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*", 10) }
            });
        }

        [Fact]
        public void OptionCanBeColonSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o:*"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*", 10)}
            });
        }

        [Fact]
        public void OptionCanBeEqualsSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o=*"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*", 10) }
            });
        }

        [Fact]
        public void DoesNotModifySpecialCharactersInArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "~!@#$%^&*()_= +[]\\{} |;':\",./<>?"}},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe("~!@#$%^&*()_= +[]\\{} |;':\",./<>?")
                }
            });
        }

        [Fact]
        public void BracketsShouldBeRetainedInText()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "[some (parenthesis) {curly} and [bracketed] text]"}},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe("[some (parenthesis) {curly} and [bracketed] text]")
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
        public void NegativeNumbersCanBeUsedForArgumentValues()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Add -b -10 -2 -3" },
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(-2,-3,"+",-10)
                }
            });
        }

        private class App
        {
            public void Add(int x, int y, 
                [Option(ShortName = "o")] string @operator = "+",
                [Option(ShortName = "b")] int @base = 10)
            {
            }

            public void Do([Operand] string arg)
            {
            }
        }
    }
}