using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_With_Backslash_Clubbing_Tests
    {
        private readonly AppSettings _allowBackslash = new AppSettings { Parser = { AllowBackslashOptionPrefix = true } };

        public Option_With_Backslash_Clubbing_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void All_clubbed_options_except_last_must_be_flags()
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = "Do /axy lala" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "'x' expects a value so it must be the last option specified in '/axy'" }
                    }
                });
        }

        [Fact]
        public void Last_clubbed_option_can_be_assigned_a_value()
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = "Do /abx lala" },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true, true, "lala", null)
                    }
                });
        }

        [Theory]
        [InlineData('/', '/')]
        [InlineData('/', '-')]
        [InlineData('-', '/')]
        public void Last_clubbed_option_can_be_assigned_a_value_that_looks_like_options(char prefix1, char prefix2)
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = $"Do {prefix1}ax {prefix2}ab" },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true,null,$"{prefix2}ab",null)
                    }
                });
        }

        public class App
        {
            public void Do(
                [Option(ShortName = "a")] bool optionA,
                [Option(ShortName = "b")] bool optionB,
                [Option(ShortName = "x")] string optionX,
                [Option(ShortName = "y")] string optionY)
            {
            }
        }
    }
}