using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Utils
{
    public static class AppRunnerTestExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner, 
            string[] args,
            ITestOutputHelper testOutputHelper,
            IEnumerable<object> dependencies = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            var testConsole = new TestConsole(
                onReadLine,
                pipedInput == null
                    ? (Func<TestConsole, string>) null
                    : console => pipedInput?.ToCsv(Environment.NewLine));
                    
            runner.UseConsole(testConsole);

            var resolver = new TestDependencyResolver();
            foreach (var dependency in dependencies ?? Enumerable.Empty<object>())
            {
                resolver.Register(dependency);
            }
            runner.UseDependencyResolver(resolver);

            var outputs = new TestOutputs();
            resolver.Register(outputs);

            var exitCode = runner.Run(args);
            var consoleOut = testConsole.Joined.ToString();
            
            // output to console to help debugging failed tests
            testOutputHelper?.WriteLine(consoleOut);

            return new AppRunnerResult(exitCode, consoleOut, outputs);
        }
    }
}