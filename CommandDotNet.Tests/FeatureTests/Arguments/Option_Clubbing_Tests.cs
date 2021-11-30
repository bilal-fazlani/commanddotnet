using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_Clubbing_Tests
    {
        public Option_Clubbing_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void All_clubbed_options_except_last_must_be_flags()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -axy lala" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "'x' expects a value so it must be the last option specified in '-axy'" }
                    }
                });
        }

        [Fact]
        public void When_a_letter_is_not_a_short_name_the_option_is_not_recognized()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -amy lala" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized command or argument '-amy'" }
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Do -abx lala"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true, true, "lala", null)
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value_that_looks_like_options()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -ax -ab" },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true,null,"-ab",null)
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value_unless_it_is_a_flag()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -ab true" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized command or argument 'true'" }
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value_with_equals_separator()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -abx=lala" },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true, true, "lala", null)
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value_with_colon_separator()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Do -abx:lala" },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true, true, "lala", null)
                    }
                });
        }

        private class App
        {
            public void Do(
                [Option('a')] bool? optionA,
                [Option('b')] bool? optionB,
                [Option('x')] string? optionX,
                [Option('y')] string? optionY)
            {
            }

            public void NeverDoThis(
                [Option('a')] bool? optionA,
                [Option('b')] bool? optionB,
                [Option] bool? ab)
            {

            }
        }
    }
}