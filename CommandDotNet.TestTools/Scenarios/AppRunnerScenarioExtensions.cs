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
        public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario, 
            Action<string> logLine = null, TestConfig config = null)
        {
            return appRunner.Verify(logLine, config, scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static AppRunnerResult Verify(this AppRunner appRunner, Action<string> logLine, TestConfig config, IScenario scenario)
        {
            if (scenario.WhenArgs != null && scenario.WhenArgsArray != null)
            {
                throw new InvalidOperationException($"Both {nameof(scenario.WhenArgs)} and {nameof(scenario.WhenArgsArray)} were specified.  Only one can be specified.");
            }

            logLine = logLine ?? Console.WriteLine;
            config = config ?? TestConfig.Default;

            AppRunnerResult results = null;
            var args = scenario.WhenArgsArray ?? scenario.WhenArgs.SplitArgs();

            var origOnSuccess = config.OnSuccess;
            if (!config.OnError.CaptureAndReturnResult)
            {
                config = config.Where(c =>
                {
                    // silence success in RunInMem so results are not printed
                    // twice when asserts fail below.
                    // success will be replaced and printed again.
                    c.OnSuccess = TestConfig.Silent.OnSuccess;
                    c.OnError.CaptureAndReturnResult = true;
                });
            }
            results = appRunner.RunInMem(
                args,
                logLine,
                scenario.Given.OnReadLine,
                scenario.Given.PipedInput,
                scenario.Given.OnPrompt,
                config);

            config.OnSuccess = origOnSuccess;

            try
            {
                AssertExitCodeAndErrorMessage(scenario, results);

                if (scenario.Then.Output != null)
                {
                    results.OutputShouldBe(scenario.Then.Output);
                }

                if (scenario.Then.Captured.Count > 0)
                {
                    AssertCapturedItems(scenario, results);
                }

                results.LogResult(logLine);
            }
            catch (Exception e)
            {
                if (!config.Source.IsNullOrWhitespace())
                {
                    logLine("");
                    logLine($"TestConfig source:{config.Source}");
                }
                results.LogResult(logLine, onError: true);
                throw;
            }

            return results;
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var sb = new StringBuilder();
            AssertExitCode(scenario, result, sb);
            AssertMissingOutputTexts(scenario, result, sb);
            AssertUnexpectedOutputTexts(scenario, result, sb);
            if (sb.Length > 0)
            {
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

        private static void AssertMissingOutputTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
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

        private static void AssertUnexpectedOutputTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
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