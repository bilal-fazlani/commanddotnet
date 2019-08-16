using System;
using System.Collections.Generic;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            ITestOutputHelper output,
            IEnumerable<object> dependencies = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output?.AsLogger(), dependencies, onReadLine, pipedInput);
        }

        public static void VerifyScenario(this AppRunner appRunner, ITestOutputHelper output, IScenario scenario)
        {
            appRunner.VerifyScenario(output.AsLogger(), scenario);
        }

        public static ILogger AsLogger(this ITestOutputHelper testOutputHelper)
        {
            return new Logger(testOutputHelper.WriteLine);
        }

    }
}