using System.Text;
using CommandDotNet.Tests.BddTests.TestScenarios;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace CommandDotNet.Tests.BddTests
{
    public class ScenarioTests
    {
        [Theory]
        [ClassData(typeof(DefaultHelpAndVersionDetailedHelpScenarios))]
        [ClassData(typeof(SingleCommandAppDetailedHelpScenarios))]
        public void Help(IScenario scenario)
        {
            var results = scenario.AppType.RunAppInMem(scenario.WhenArgs);
            AssertExitCodeAndErrorMessage(scenario, results);

            results.HelpShouldBe(scenario.Then.Help);
        }

        [Theory]
        [ClassData(typeof(BasicParseScenarios))]
        public void Parse(IScenario scenario)
        {
            var results = scenario.AppType.RunAppInMem(scenario.WhenArgs);
            AssertExitCodeAndErrorMessage(scenario, results);
            AssertOutputItems(scenario, results);
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var expectedExitCode = scenario.Then.ExitCode.GetValueOrDefault();
            var expectedOutputText = scenario.Then.HelpContainsText;

            if (expectedExitCode != result.ExitCode || (expectedOutputText != null && !result.ConsoleOut.Contains(expectedOutputText)))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"ExitCode: expected={expectedExitCode} actual={result.ExitCode}");
                if (expectedOutputText != null)
                {
                    sb.AppendLine($"Expected error message in output:");
                    sb.AppendLine();
                    sb.AppendLine(expectedOutputText);
                }

                sb.AppendLine();
                sb.AppendLine("Console output:");

                sb.AppendLine(string.IsNullOrWhiteSpace(result.ConsoleOut) ? "<no output>" : result.ConsoleOut);

                throw new AssertionFailedException(sb.ToString());
            }
        }

        private static void AssertOutputItems(IScenario scenario, AppRunnerResult results)
        {
            foreach (var expectedOutput in scenario.Then.Outputs)
            {
                var actualOutput = results.TestOutputs.Get(expectedOutput.GetType());
                actualOutput.Should()
                    .NotBeNull(because: $"{expectedOutput.GetType().Name} should have been output in test run");
                actualOutput.Should().BeEquivalentTo(expectedOutput);
            }
        }
    }
}