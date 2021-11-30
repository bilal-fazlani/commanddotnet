using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_With_Backslash_Tests
    {
        private readonly AppSettings _allowBackslash = new() { Parser = { AllowBackslashOptionPrefix = true } };

        public Option_With_Backslash_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void Backslash_does_not_change_help()
        {
            new AppRunner<App>(_allowBackslash)
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
        public void Backslash_can_be_used_with_long_names()
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = "Do /flag /value lala" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala")}
                });
        }

        [Fact]
        public void Backslash_can_be_used_with_short_names()
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = "Do /f /v lala" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala") }
                });
        }

        [Fact]
        public void DoubleHyphen_can_still_be_used_with_long_names()
        {
            new AppRunner<App>(_allowBackslash)
                .Verify(new Scenario
                {
                    When = { Args = "Do --flag --value lala" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(true, "lala") }
                });
        }

        [Fact]
        public void SingleHyphen_can_still_be_used_with_short_names()
        {
            new AppRunner<App>(_allowBackslash)
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