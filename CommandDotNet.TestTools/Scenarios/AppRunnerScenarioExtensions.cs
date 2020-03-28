using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace CommandDotNet.TestTools.Scenarios
{
    public static class AppRunnerScenarioExtensions
    {
        /// <summary>Run and verify the scenario expectations, output results to <see cref="Console"/></summary>
        public static AppRunnerResult VerifyScenario(this AppRunner appRunner, IScenario scenario)
        {
            return appRunner.VerifyScenario(new Logger(Console.WriteLine), scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static AppRunnerResult VerifyScenario(this AppRunner appRunner, ILogger logger, IScenario scenario)
        {
            if (scenario.WhenArgs != null && scenario.WhenArgsArray != null)
            {
                throw new InvalidOperationException($"Both {nameof(scenario.WhenArgs)} and {nameof(scenario.WhenArgsArray)} were specified.  Only one can be specified.");
            }

            AppRunnerResult results = null;
            var args = scenario.WhenArgsArray ?? scenario.WhenArgs.SplitArgs();
            try
            {
                results = appRunner.RunInMem(
                    args,
                    logger,
                    scenario.Given.OnReadLine,
                    scenario.Given.PipedInput,
                    scenario.Given.OnPrompt,
                    returnResultOnError: true);

                AssertExitCodeAndErrorMessage(scenario, results);

                if (scenario.Then.Result != null)
                {
                    results.OutputShouldBe(scenario.Then.Result);
                }

                if (scenario.Then.Outputs.Count > 0)
                {
                    AssertOutputItems(scenario, results);
                }

                return results;
            }
            catch (Exception e)
            {
                logger.WriteLine(scenario.ToString());
                logger.WriteLine("");
                PrintContext(appRunner, logger);
                if (results != null)
                {
                    logger.WriteLine("");
                    logger.WriteLine("App Results:");
                    logger.WriteLine(results.ConsoleOutAndError);
                }
                throw;
            }
        }

        private static void PrintContext(AppRunner appRunner, ILogger logger)
        {
            logger.WriteLine("");
            logger.WriteLine(appRunner.ToString());
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var sb = new StringBuilder();

            AssertExitCode(scenario, result, sb);

            AssertMissingHelpTexts(scenario, result, sb);

            AssertUnexpectedHelpTexts(scenario, result, sb);

            if (sb.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Console output <begin> ------------------------------");
                sb.AppendLine(String.IsNullOrWhiteSpace(result.ConsoleOutAndError) ? "<no output>" : result.ConsoleOutAndError);
                sb.AppendLine("Console output <end>   ------------------------------");

                throw new AssertionFailedException(sb.ToString());
            }
        }

        private static void AssertExitCode(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var expectedExitCode = scenario.Then.ExitCode.GetValueOrDefault();
            if (expectedExitCode != result.ExitCode)
            {
                sb.AppendLine($"ExitCode: expected={expectedExitCode} actual={result.ExitCode}");
            }
        }

        private static void AssertMissingHelpTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var missingHelpTexts = scenario.Then.ResultsContainsTexts
                .Where(t => !result.OutputContains(t))
                .ToList();
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
        }

        private static void AssertUnexpectedHelpTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var unexpectedHelpTexts = scenario.Then.ResultsNotContainsTexts
                .Where(result.OutputContains)
                .ToList();
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