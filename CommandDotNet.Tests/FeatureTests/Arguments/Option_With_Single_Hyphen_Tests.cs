using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_With_Single_Hyphen_Tests
    {
        private readonly AppSettings _allowSingleHyphenForLongNames = new() { Parser = { AllowSingleHyphenForLongNames = true } };

        public Option_With_Single_Hyphen_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void SingleHyphen_does_not_change_help()
        {
            new AppRunner<App>(_allowSingleHyphenForLongNames)
                .Verify(new Scenario
                {
                    When = { Args = "Do -h" },
                    Then = { Output = @"Usage: testhost.dll Do [options]

Options:

  -f | --flag

  -v | --value  <TEXT>" }
                });
        }

        [Fact]
        public void SingleHyphen_can_be_used_with_long_names()
        {
            new AppRunner<App>(_allowSingleHyphenForLongNames)
                .Verify(new Scenario
                {
                    When = { Args = "Do -flag -value lala" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala") }
                });
        }

        [Fact]
        public void SingleHyphen_can_be_used_with_short_names()
        {
            new AppRunner<App>(_allowSingleHyphenForLongNames)
                .Verify(new Scenario
                {
                    When = { Args = "Do -f -v lala" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala") }
                });
        }

        private class App
        {
            public void Do(
                [Option('f')] bool flag,
                [Option('v')] string value)
            {
            }
        }
    }
}