using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class ArgumentSeparatorTests
    {
        private readonly ITestOutputHelper _output;

        public ArgumentSeparatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Given_ParseSeparated_Disabled_When_OperandValueWithDash_FailsWithUnrecognizedOption()
        {
            new AppRunner<Math>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -1 -3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '-1'" }
                    }
                });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByAppSetting_When_OperandValueWithDash_FailsWithUnrecognizedOption()
        {
            var appSettings = new AppSettings { ParseSeparatedArguments = true };
            new AppRunner<Math>(appSettings)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -1 -3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '-1'" }
                    }
                });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByCommand_When_OperandValueWithDash_FailsWithUnrecognizedOption()
        {
            new AppRunner<Math>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_Enabled -1 -3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '-1'" }
                    }
                });
        }

        [Fact]
        public void Given_ParseSeparated_DisabledByAppSetting_When_Separator_OperandValueWithDash_OperandsAreIgnored()
        {
            var result = new AppRunner<Math>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -- -1 -3",
                    Then = { Result = "0" }
                });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3" });
        }

        [Fact]
        public void Given_ParseSeparated_DisabledByCommand_When_Separator_OperandValueWithDash_OperandsAreIgnored()
        {
            var appSettings = new AppSettings { ParseSeparatedArguments = true };
            var result = new AppRunner<Math>(appSettings)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_Disabled -- -1 -3",
                    Then = { Result = "0" }
                });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3" });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByAppSetting_When_Separator_OperandValueWithDash_OperandsAreParsed()
        {
            var appSettings = new AppSettings {ParseSeparatedArguments = true};
            var result = new AppRunner<Math>(appSettings)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -- -1 -3",
                    Then = { Result = "-4" }
                });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3" });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByCommand_When_Separator_OperandValueWithDash_OperandsAreParsed()
        {
            var result = new AppRunner<Math>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_Enabled -- -1 -3",
                    Then = { Result = "-4" }
                });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3" });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByAppSetting_And_IgnoreUnexpected_Enabled_When_Separator_OperandValueWithDash_OperandsAreParsed_And_ExtraArgsAreIgnoredAndCaptured()
        {
            var appSettings = new AppSettings {IgnoreUnexpectedOperands = true, ParseSeparatedArguments = true};
            var result = new AppRunner<Math>(appSettings)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -- -1 -3 -5 -7",
                    Then = { Result = "-4" }
                });

            result.CommandContext.ParseResult.RemainingOperands
                .Should().BeEquivalentTo(new[] { "-5", "-7" });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3", "-5", "-7" });
        }

        [Fact]
        public void Given_ParseSeparated_EnabledByCommand_And_IgnoreUnexpected_Enabled_When_Separator_OperandValueWithDash_OperandsAreParsed_And_ExtraArgsAreIgnoredAndCaptured()
        {
            var appSettings = new AppSettings { IgnoreUnexpectedOperands = true };
            var result = new AppRunner<Math>(appSettings)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add_Enabled -- -1 -3 -5 -7",
                    Then = { Result = "-4" }
                });

            result.CommandContext.ParseResult.RemainingOperands
                .Should().BeEquivalentTo(new[] { "-5", "-7" });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3", "-5", "-7" });
        }

        public class Math
        {
            public void Add(IConsole console, int x, int y)
            {
                console.WriteLine(x + y);
            }

            [Command(ParseSeparatedArguments = true)]
            public void Add_Enabled(IConsole console, int x, int y)
            {
                console.WriteLine(x + y);
            }

            [Command(ParseSeparatedArguments = false)]
            public void Add_Disabled(IConsole console, int x, int y)
            {
                console.WriteLine(x + y);
            }
        }
    }
}