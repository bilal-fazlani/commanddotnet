using System;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Tests.BddTests.TestScenarios;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.BddTests
{
    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [ClassData(typeof(BasicHelpAndVersionDetailedHelpScenarios))]
        [ClassData(typeof(SingleCommandAppDetailedHelpScenarios))]
        [ClassData(typeof(DefaultCommandDetailedHelpScenarios))]
        [ClassData(typeof(ArgumentSeparatorHelpScenarios))]
        public void Help(IScenario scenario)
        {
            try
            {

                var results = scenario.AppType.RunAppInMem(scenario.WhenArgs, scenario.And.AppSettings);
                AssertExitCodeAndErrorMessage(scenario, results);

                if (scenario.Then.Help != null)
                {
                    results.HelpShouldBe(scenario.Then.Help);
                }
            }
            catch (Exception)
            {
                PrintContext(scenario);
                throw;
            }
        }

        [Theory]
        [ClassData(typeof(BasicParseScenarios))]
        [ClassData(typeof(DefaultCommandParseScenarios))]
        [ClassData(typeof(ArgumentSeparatorParseScenarios))]
        public void Parse(IScenario scenario)
        {
            try
            {
                var results = scenario.AppType.RunAppInMem(scenario.WhenArgs, scenario.And.AppSettings);
                AssertExitCodeAndErrorMessage(scenario, results);

                AssertOutputItems(scenario, results);
            }
            catch (Exception)
            {
                PrintContext(scenario);
                throw;
            }
        }

        private void PrintContext(IScenario scenario)
        {
            _output.WriteLine($"Scenario class: {scenario.Context.Host.GetType()}");
            var appSettings = scenario.And.AppSettings;
            var appSettingsProps = appSettings.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(p => p.Name);
            _output.WriteLine("");
            _output.WriteLine($"AppSettings:");
            foreach (var propertyInfo in appSettingsProps)
            {
                _output.WriteLine($"  {propertyInfo.Name}: {propertyInfo.GetValue(appSettings)}");
            }
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var expectedExitCode = scenario.Then.ExitCode.GetValueOrDefault();
            var missingHelpTexts = scenario.Then.HelpContainsTexts
                .Where(t => !result.ConsoleOut.Contains(t))
                .ToList();

            if (expectedExitCode != result.ExitCode || missingHelpTexts.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"ExitCode: expected={expectedExitCode} actual={result.ExitCode}");
                if (missingHelpTexts.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Missing text in output:");
                    foreach (var text in missingHelpTexts)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"  {text}");
                    }
                }

                sb.AppendLine();
                sb.AppendLine("Console output:");
                sb.AppendLine();
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