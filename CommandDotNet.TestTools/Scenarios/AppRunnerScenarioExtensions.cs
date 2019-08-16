using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace CommandDotNet.TestTools.Scenarios
{
    public static class AppRunnerScenarioExtensions
    {
        /// <summary>Run and verify the scenario expectations, output results to <see cref="Console"/></summary>
        public static void VerifyScenario(this AppRunner appRunner, IScenario scenario)
        {
            appRunner.VerifyScenario(new Logger(Console.WriteLine), scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static void VerifyScenario(this AppRunner appRunner, ILogger logger, IScenario scenario)
        {
            if (scenario.WhenArgs != null && scenario.WhenArgsArray != null)
            {
                throw new InvalidOperationException($"Both {nameof(scenario.WhenArgs)} and {nameof(scenario.WhenArgsArray)} were specified.  Only one can be specified.");
            }

            AppRunnerResult results = null;
            try
            {
                // scenarios don't pass testOutputHelper
                // RunInMem will print to AppRunnerResult.ConsoleOut
                // The output will be printed only if there is
                // an exception to be debugged and after all other
                // relevant context is printed.
                results = appRunner.RunInMem(
                    scenario.WhenArgsArray ?? scenario.WhenArgs.SplitArgs(),
                    null,
                    scenario.Given.OnReadLine,
                    scenario.Given.PipedInput);

                AssertExitCodeAndErrorMessage(scenario, results);

                if (scenario.Then.Result != null)
                {
                    results.OutputShouldBe(scenario.Then.Result);
                }

                if (scenario.Then.Outputs.Count > 0)
                {
                    AssertOutputItems(scenario, results);
                }
            }
            catch (Exception)
            {
                PrintContext(appRunner, logger, scenario);
                if (results != null)
                {
                    logger.WriteLine("");
                    logger.WriteLine("App Results:");
                    logger.WriteLine(results.ConsoleOut);
                }
                throw;
            }
        }

        private static void PrintContext(AppRunner appRunner, ILogger logger, IScenario scenario)
        {
            var appSettings = appRunner.AppSettings;
            var appSettingsProps = appSettings.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(p => p.Name);
            logger.WriteLine("");
            logger.WriteLine($"AppSettings:");
            foreach (var propertyInfo in appSettingsProps)
            {
                logger.WriteLine($"  {propertyInfo.Name}: {propertyInfo.GetValue(appSettings)}");
            }
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var expectedExitCode = scenario.Then.ExitCode.GetValueOrDefault();
            var missingHelpTexts = scenario.Then.ResultsContainsTexts
                .Where(t => !result.OutputContains(t))
                .ToList();

            var unexpectedHelpTexts = scenario.Then.ResultsNotContainsTexts
                .Where(result.OutputContains)
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
                sb.AppendLine("Console output <begin> ------------------------------");
                sb.AppendLine(String.IsNullOrWhiteSpace(result.ConsoleOut) ? "<no output>" : result.ConsoleOut);
                sb.AppendLine("Console output <end>   ------------------------------");

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
                var expectedOutputTypes = new HashSet<Type>(scenario.Then.Outputs.Select(o => o.GetType()));
                var unexpectedTypes = actualOutputs.Keys
                    .Where(t => !expectedOutputTypes.Contains(t))
                    .ToOrderedCsv();

                throw new AssertionFailedException($"Unexpected output: {unexpectedTypes}");
            }
        }
    }
}