using System;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public class ScenarioVerifier
    {
        private readonly ITestOutputHelper _output;

        public ScenarioVerifier(ITestOutputHelper output)
        {
            _output = output;
        }

        public void VerifyScenario(IScenario s)
        {
            try
            {
                var results = s.AppType.RunAppInMem(s.WhenArgs, s.And.AppSettings);
                AssertExitCodeAndErrorMessage(s, results);

                if (s.Then.Result != null)
                {
                    results.HelpShouldBe(s.Then.Result);
                }

                if (s.Then.Outputs.Count > 0)
                {
                    AssertOutputItems(s, results);
                }
            }
            catch (Exception)
            {
                PrintContext(s);
                throw;
            }
        }

        private void PrintContext(IScenario scenario)
        {
            if (scenario.Context != null)
            {
                _output.WriteLine($"Scenario class: {scenario.Context.Host.GetType()}");
            }
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
            var missingHelpTexts = scenario.Then.ResultsContainsTexts
                .Where(t => !result.HelpContains(t))
                .ToList();

            var unexpectedHelpTexts = scenario.Then.ResultsNotContainsTexts
                .Where(result.HelpContains)
                .ToList();

            if (expectedExitCode != result.ExitCode || missingHelpTexts.Count > 0 || unexpectedHelpTexts.Count > 0)
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
                if (unexpectedHelpTexts.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Unexpected text in output:");
                    foreach (var text in unexpectedHelpTexts)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"  {text}");
                    }
                }

                sb.AppendLine();
                sb.AppendLine("Console output:");
                sb.AppendLine();
                sb.AppendLine(String.IsNullOrWhiteSpace(result.ConsoleOut) ? "<no output>" : result.ConsoleOut);

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

            var actualOutputs = results.TestOutputs.Outputs;
            if (!scenario.Then.AllowUnspecifiedOutputs && actualOutputs.Count > scenario.Then.Outputs.Count)
            {
                var expectedOutputTypes = scenario.Then.Outputs.Select(o => o.GetType()).ToHashSet();
                var unexpectedTypes = String.Join(",", actualOutputs.Keys
                    .Where(t => !expectedOutputTypes.Contains(t))
                    .Select(t => t.Name)
                    .OrderBy(n => n));

                throw new AssertionFailedException($"Unexpected output: {unexpectedTypes}");
            }
        }
    }
}