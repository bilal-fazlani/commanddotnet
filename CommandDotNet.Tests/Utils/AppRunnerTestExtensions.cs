using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Tests.Utils
{
    public static class AppRunnerTestExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner, 
            string[] args,
            ILogger logger,
            IEnumerable<object> dependencies = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            var testConsole = new TestConsole(
                onReadLine,
                pipedInput == null
                    ? (Func<TestConsole, string>) null
                    : console => pipedInput?.ToCsv(Environment.NewLine));
                    
            runner.Configure(c => c.UseConsole(testConsole));

            var resolver = new TestDependencyResolver();
            dependencies?.ForEach(resolver.Register);
            runner.UseDependencyResolver(resolver, useLegacyInjectDependenciesAttribute: true);

            var outputs = new TestOutputs();
            resolver.Register(outputs);

            var exitCode = runner.Run(args);
            var consoleOut = testConsole.Joined.ToString();
            
            // output to console to help debugging failed tests
            logger?.WriteLine(consoleOut);

            return new AppRunnerResult(exitCode, consoleOut, outputs);
        }
    }
}