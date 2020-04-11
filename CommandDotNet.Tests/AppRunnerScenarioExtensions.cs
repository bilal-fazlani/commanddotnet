using System;
using System.Collections.Generic;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult VerifyScenario(this AppRunner appRunner, ITestOutputHelper output, IScenario scenario)
        {
            return appRunner.VerifyScenario(output.WriteLine, scenario);
        }
    }
}