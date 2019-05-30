using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Tests.BddTests.Framework;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.BddTests
{
    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        private static List<ScenariosBaseTheory> AllScenarios = typeof(ScenarioTests).Assembly.GetTypes()
            .Where(t => typeof(ScenariosBaseTheory).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ScenariosBaseTheory>()
            .ToList();

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> ActiveScenarios =>
            AllScenarios
                .SelectMany(s => s.GetActive())
                .ToObjectArrays();

        public static IEnumerable<object[]> SkippedScenarios
        {
            get
            {
                var skippedScenarios = AllScenarios
                    .SelectMany(s => s.GetSkipped())
                    .ToList();

                return skippedScenarios.Count == 0
                    ? new[] {new Skipped()}.ToObjectArrays()
                    : skippedScenarios.ToObjectArrays();
            }
        }


        [Theory]
        [MemberData(nameof(SkippedScenarios), Skip = "skipped scenarios")]
        public void Skipped(IScenario scenario)
        {
        }


        [Theory]
        [MemberData(nameof(ActiveScenarios))]
        public void Scenarios(IScenario s)
        {
            // short parameter name to reduce redundant appearance of "scenario" in test name.
            try
            {
                var results = s.AppType.RunAppInMem(s.WhenArgs, s.And.AppSettings);
                AssertExitCodeAndErrorMessage(s, results);

                if (s.Then.Help != null)
                {
                    results.HelpShouldBe(s.Then.Help);
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
                .Where(t => !result.HelpContains(t))
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

            var actualOutputs = results.TestOutputs.Outputs;
            if (scenario.Then.OutputsStrict && actualOutputs.Count > scenario.Then.Outputs.Count)
            {
                var expectedOutputTypes = scenario.Then.Outputs.Select(o => o.GetType()).ToHashSet();
                var unexpectedTypes = string.Join(",", actualOutputs.Keys
                    .Where(t => !expectedOutputTypes.Contains(t))
                    .Select(t => t.Name)
                    .OrderBy(n => n));

                throw new AssertionFailedException($"Unexpected output: {unexpectedTypes}");
            }
        }
    }
}