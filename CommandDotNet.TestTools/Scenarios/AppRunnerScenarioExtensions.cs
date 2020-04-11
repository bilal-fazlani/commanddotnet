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
            return appRunner.Verify(null, scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static AppRunnerResult VerifyScenario(this AppRunner appRunner, Action<string> logLine, IScenario scenario)
        {
            return appRunner.Verify(null, scenario);
        }

        /// <summary>Run and verify the scenario expectations, output results to <see cref="Console"/></summary>
            public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
        {
            return appRunner.Verify(null, scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static AppRunnerResult Verify(this AppRunner appRunner, Action<string> logLine, IScenario scenario)
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
                    logLine,
                    scenario.Given.OnReadLine,
                    scenario.Given.PipedInput,
                    scenario.Given.OnPrompt,
                    returnResultOnError: true);

                AssertExitCodeAndErrorMessage(scenario, results);

                if (scenario.Then.Output != null)
                {
                    results.OutputShouldBe(scenario.Then.Output);
                }

                if (scenario.Then.Captured.Count > 0)
                {
                    AssertCapturedItems(scenario, results);
                }

                return results;
            }
            catch (Exception e)
            {
                logLine(scenario.ToString());
                logLine("");
                PrintContext(appRunner, logLine);
                if (results != null)
                {
                    logLine("");
                    logLine("App Results:");
                    logLine(results.ConsoleOutAndError);
                }
                throw;
            }
        }

        private static void PrintContext(AppRunner appRunner, Action<string> logLine)
        {
            logLine("");
            logLine(appRunner.ToString());
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
            var missingHelpTexts = scenario.Then.OutputContainsTexts
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
            var unexpectedHelpTexts = scenario.Then.OutputNotContainsTexts
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

        private static void AssertCapturedItems(IScenario scenario, AppRunnerResult results)
        {
            foreach (var expectedOutput in scenario.Then.Captured)
            {
                var actualOutput = results.TestCaptures.Get(expectedOutput.GetType());
                actualOutput.Should()
                    .NotBeNull(because: $"{expectedOutput.GetType().Name} should have been captured in the test run but wasn't");
                actualOutput.Should().BeEquivalentTo(expectedOutput);
            }

            var actualOutputs = results.TestCaptures.Captured;
            if (!scenario.Then.AllowUnspecifiedCaptures && actualOutputs.Count > scenario.Then.Captured.Count)
            {
                var expectedOutputTypes = new HashSet<Type>(scenario.Then.Captured.Select(o => o.GetType()));
                var unexpectedTypes = actualOutputs.Keys
                    .Where(t => !expectedOutputTypes.Contains(t))
                    .ToOrderedCsv();

                throw new AssertionFailedException($"Unexpected captures: {unexpectedTypes}");
            }
        }
    }
}