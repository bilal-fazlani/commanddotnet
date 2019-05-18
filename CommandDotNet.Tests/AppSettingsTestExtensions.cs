using System.IO;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests
{
    public static class AppSettingsTestExtensions
    {
        public static AppRunnerResult RunInMem<T>(this AppRunner<T> runner, params string[] args) where T : class
        {
            var consoleOut = new StringWriter();
            runner.OverrideConsoleOut(consoleOut);
            runner.OverrideConsoleError(consoleOut);

            var resolver = new TestDependencyResolver();
            resolver.Register(new TestWriter(consoleOut));
            runner.DependencyResolver = resolver;
            
            var inputs = new Inputs();
            resolver.Register(inputs);
            
            var exitCode = runner.Run(args);
            return new AppRunnerResult(exitCode, consoleOut.ToString(), inputs);
        }
    }
}