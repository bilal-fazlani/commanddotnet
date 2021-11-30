using System;
using System.Collections.Generic;
using CommandDotNet.TestTools.Scenarios;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    public static class AssertionExtensions
    {
        public static AppRunner Expect<TEx>(this AppRunner appRunner,
            Action<TEx>? assertEx = null)
            where TEx : Exception
        {
            const int exceptionAsserted = int.MinValue + 1234;

            return appRunner
                .UseErrorHandler((ctx, ex) =>
                {
                    if (ex is not TEx)
                    {
                        throw new AssertFailedException($"exception {ex.GetType()} is not of type {typeof(TEx)}");
                    }
                    assertEx?.Invoke((TEx)ex);
                    return exceptionAsserted;
                })
                .AssertAfterRun(r =>
                {
                    if (r.ExitCode != exceptionAsserted)
                    {
                        throw new AssertFailedException($"exception {typeof(TEx)} was not raised in the test run. {nameof(appRunner.UseErrorHandler)}");
                    }
                });
        }

        internal static AppRunnerResult VerifyAfterRunAssertions(this AppRunnerResult result)
        {
            result.Runner.AppConfig!.Services
                .GetOrDefault<List<Action<AppRunnerResult>>>()
                ?.ForEach(a => a(result));
            return result;
        }

        internal static AppRunner AfterRun(this AppRunner appRunner, Action<AppRunnerResult> action)
        {
            return appRunner.Configure(cfg =>
            {
                var postRunActions = cfg.Services.GetOrAdd(() => new List<Action<AppRunnerResult>>());
                postRunActions.Add(action);
            });
        }

        internal static AppRunner AssertAfterRun(this AppRunner appRunner, Action<AppRunnerResult> assert)
        {
            return appRunner.Configure(cfg =>
            {
                var postRunActions = cfg.Services.GetOrAdd(() => new List<Action<AppRunnerResult>>());
                postRunActions.Add(assert);
            });
        }

        internal static void ShouldBe(this string? actual, string? expected, string valueName)
        {
            if (expected?.Equals(actual) ?? actual == null)
            {
                return;
            }

            Fail(valueName, expected, actual, GetFirstDiffIndex(expected, actual));
        }

        private static int GetFirstDiffIndex(string? expected, string? actual)
        {
            if (expected == null || actual == null) return -1;

            int length = Math.Min(expected.Length, actual.Length);

            for (int i = 0; i < length; i++)
            {
                if (expected[i] != actual[i])
                {
                    return i;
                }
            }

            return length;
        }

        private static void Fail(string valueName, string? expected, string? actual, int diffIndex)
        {
            var startIndex = Math.Max(0, diffIndex-10);
            var actualSnippet = actual?.Substring(startIndex, Math.Min(actual.Length - startIndex, diffIndex + 10));
            var expectedSnippet = expected?.Substring(startIndex, Math.Min(expected.Length - startIndex, diffIndex + 10));

            throw new AssertFailedException(
                $"Mismatch at index {diffIndex}{NewLine}" +
                $"expected:{expectedSnippet}{NewLine}" +
                $"actual  :{actualSnippet}{NewLine}" +
                $"Expected value for {valueName} to be {FormatMessage(expected)} " +
                $"but was {FormatMessage(actual)}");
        }

        private static string FormatMessage(object? item) => item == null ? "<null>" : $"{NewLine}\"{item}\"{NewLine}";
    }
}