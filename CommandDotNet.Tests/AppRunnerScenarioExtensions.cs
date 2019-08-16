using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
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
            return AppRunnerTestExtensions.RunInMem(runner, args, output?.AsLogger(), dependencies, onReadLine, pipedInput);
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