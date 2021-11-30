using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_With_Backslash_and_Single_Hyphen_Tests
    {
        private readonly AppSettings _allowBoth = new()
        {
            Parser =
            {
                AllowBackslashOptionPrefix = true,
                AllowSingleHyphenForLongNames = true
            }
        };

        public Option_With_Backslash_and_Single_Hyphen_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void SingleHyphen_does_not_change_help()
        {
            new AppRunner<App>(_allowBoth)
                .Verify(new Scenario
                {
                    When = { Args = "Do -h" },
                    Then = { Output = @"Usage: testhost.dll Do [options]

Options:

  -f | --flag

  -v | --value  <TEXT>

  -o | --other  <TEXT>" }
                });
        }

        [Fact]
        public void SingleHyphen_and_Backslash_and_DoubleHyphen_can_be_used_with_long_names()
        {
            new AppRunner<App>(_allowBoth)
                .Verify(new Scenario
                {
                    When = { Args = "Do -flag /value lala --other fishies" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala", "fishies") }
                });
        }

        [Fact]
        public void SingleHyphen_and_Backslash_can_be_used_with_short_names()
        {
            new AppRunner<App>(_allowBoth)
                .Verify(new Scenario
                {
                    When = { Args = "Do -f -v lala /o fishies" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala", "fishies") }
                });
        }

        private class App
        {
            public void Do(
                [Option('f')] bool flag,
                [Option('v')] string value,
                [Option('o')] string other)
            {
            }
        }
    }
}