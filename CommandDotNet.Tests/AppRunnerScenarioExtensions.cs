using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        [Obsolete("Use RunInMem without `output` or `logLine`")]
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output.WriteLine, onReadLine, pipedInput);
        }

        [Obsolete("Use RunInMem without `output` or `logLine`")]
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output.WriteLine, onReadLine, pipedInput);
        }

        [Obsolete("Use Verify without `output` or `logLine`")]
        public static AppRunnerResult Verify(this AppRunner appRunner, ITestOutputHelper output, IScenario scenario)
        {
            // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
            return appRunner.Verify(output.WriteLine, TestConfig.Default, scenario);
        }

        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string args,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, Ambient.Output.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, Ambient.Output.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
        {
            // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
            return appRunner.Verify(Ambient.Output.WriteLine, TestConfig.Default, scenario);
        }
    }
}