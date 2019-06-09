using System;
using System.IO;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.Utils
{
    public static class AppRunnerTestExtensions
    {
        public static AppRunnerResult RunAppInMem(this Type appType, string args, AppSettings appSettings = null)
        {
            return RunAppInMem(appType, args?.Split(' ') ?? new string[0], appSettings);
        }

        public static AppRunnerResult RunAppInMem(this Type appType, string[] args, AppSettings appSettings = null)
        {
            var type = typeof(AppRunner<>).MakeGenericType(appType);
            var runner = Activator.CreateInstance(type, appSettings ?? new AppSettings());

            var runInMemMethod = typeof(AppRunnerTestExtensions)
                .GetMethod("RunInMem")
                .GetGenericMethodDefinition()
                .MakeGenericMethod(appType);

            return (AppRunnerResult)runInMemMethod.Invoke(null, new[] { runner, args });
        }

        public static AppRunnerResult RunInMem<T>(this AppRunner<T> runner, params string[] args) where T : class
        {
            return runner.RunAppInMem(args);
        }

        private static AppRunnerResult RunAppInMem<T>(this AppRunner<T> runner, string[] args) where T : class
        {
            var consoleOut = new StringWriter();
            runner.OverrideConsoleOut(consoleOut);
            runner.OverrideConsoleError(consoleOut);

            var resolver = new TestDependencyResolver();
            resolver.Register(new TestWriter(consoleOut));
            runner.DependencyResolver = resolver;

            var inputs = new TestOutputs();
            resolver.Register(inputs);

            var exitCode = runner.Run(args);
            return new AppRunnerResult(exitCode, consoleOut.ToString(), inputs);
        }
    }
}