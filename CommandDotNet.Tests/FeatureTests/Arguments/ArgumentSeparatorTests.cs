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
        public void GivenOperandValueWithDash_FailsWithUnrecognizedOption()
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
        public void GivenOperandValueWithDash_AndArgumentSeparator_Succeeds()
        {
            var result = new AppRunner<Math>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Add -- -1 -3",
                    Then = { Result = "-4" }
                });

            result.CommandContext.ParseResult.SeparatedArguments
                .Should().BeEquivalentTo(new[] { "-1", "-3" });
        }

        [Fact]
        public void GivenOperandValueWithDash_AndArgumentSeparator_ExtraArgsAreIgnoredAndCaptured()
        {
            var result = new AppRunner<Math>(new AppSettings { IgnoreUnexpectedOperands = true })
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

        public class Math
        {
            public void Add(IConsole console, int x, int y)
            {
                console.WriteLine(x + y);
            }
        }
    }
}