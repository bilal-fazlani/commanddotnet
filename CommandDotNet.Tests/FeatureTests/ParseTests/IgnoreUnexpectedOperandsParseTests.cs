using CommandDotNet.Execution;
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

        [InlineData(false, "UseAppSettings", false)]
        [InlineData(false, "Enable", true)]
        [InlineData(false, "Disable", false)]
        [InlineData(true, "UseAppSettings", true)]
        [InlineData(true, "Enable", true)]
        [InlineData(true, "Disable", false)]
        [Theory]
        public void IgnoreUnexpectedOperands_UseCommandAttribute_DefaultFromAppSettings(
            bool appSettingsEnabled, string command, bool expectedEnabled)
        {
            var appSettings = new AppSettings { Parser = { IgnoreUnexpectedOperands = appSettingsEnabled } };
            new AppRunner<SettingsApp>(appSettings)
                .StopAfter(MiddlewareStages.ParseInput)
                .RunInMem(command)
                .CommandContext.ParseResult!.TargetCommand.IgnoreUnexpectedOperands!.Should().Be(expectedEnabled);
        }

        [Fact]
        public void Given_IgnoreExtraOperands_Disabled_Parse_ThrowsUnrecognized()
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
        public void Given_IgnoreExtraOperands_CollectsRemaining()
        {
            var results = new AppRunner<App>(new AppSettings { Parser = {IgnoreUnexpectedOperands = true} })
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

        private class App
        {
            public void Add(int x, int y, [Option('o')] string @operator = "+")
            {
            }
        }

        public class SettingsApp
        {
            public void UseAppSettings()
            {
            }

            [Command(IgnoreUnexpectedOperands = true)]
            public void Enable(int x, int y)
            {
            }

            [Command(IgnoreUnexpectedOperands = false)]
            public void Disable(int x, int y)
            {
            }
        }
    }
}