using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests
{
    public class IgnoreUnexpectedOperandsParseTests
    {
        public IgnoreUnexpectedOperandsParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByAppSetting_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Add 2 3 4" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"},
                        AssertContext = ctx => ctx.ParseResult!.RemainingOperands.Should().BeEmpty()
                    }
                });
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByCommand_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>(new AppSettings { IgnoreUnexpectedOperands = true })
                .Verify(new Scenario
                {
                    When = { Args = "Add_DisabledIgnore 2 3 4" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"},
                        AssertContext = ctx => ctx.ParseResult!.RemainingOperands.Should().BeEmpty()
                    }
                });
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByAppSettings_CollectsRemaining()
        {
            var results = new AppRunner<App>(new AppSettings { IgnoreUnexpectedOperands = true })
                .Verify(new Scenario
                {
                    When = { Args = "Add 2 3 4" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(2, 3, "+");
                            ctx.ParseResult!.RemainingOperands.Should().BeEquivalentTo("4");
                        }
                    }
                });
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByCommand_CollectsRemaining()
        {
            var results = new AppRunner<App>()
                .Verify(scenario: new Scenario
                {
                    When = { Args = "Add_EnabledIgnore 2 3 4" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(2, 3);
                            ctx.ParseResult!.RemainingOperands.Should().BeEquivalentTo("4");
                        }
                    }
                });
        }

        private class App
        {
            public void Add(int x, int y, [Option(ShortName = "o")] string @operator = "+")
            {
            }

            [Command(IgnoreUnexpectedOperands = true)]
            public void Add_EnabledIgnore(int x, int y)
            {
            }

            [Command(IgnoreUnexpectedOperands = false)]
            public void Add_DisabledIgnore(int x, int y)
            {
            }
        }
    }
}