using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Models;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Utils
{
    public static class AppRunnerTestExtensions
    {
        private static readonly MethodInfo RunAppInMemoryGenericMethod = typeof(AppRunnerTestExtensions)
            .GetMethods()
            .First(m => m.Name == nameof(RunInMem) && m.GetParameters().Any(p => p.Name == "args" && p.ParameterType == typeof(string[])))
            .GetGenericMethodDefinition();

        public static AppRunnerResult RunAppInMem(
            this Type appType, string[] args, 
            AppSettings appSettings = null, IEnumerable<object> dependencies = null)
        {
            var type = typeof(AppRunner<>).MakeGenericType(appType);
            var runner = Activator.CreateInstance(type, appSettings ?? TestAppSettings.TestDefault);

            var runInMemMethod = RunAppInMemoryGenericMethod.MakeGenericMethod(appType);

            // scenarios don't pass testOutputHelper because that framework
            // print the AppRunnerResult.ConsoleOut so it's not necessary
            // to capture output directly to XUnit
            return (AppRunnerResult)runInMemMethod.Invoke(null, new[] { runner, args, null, dependencies });
        }

        public static AppRunnerResult RunInMem<T>(
            this AppRunner<T> runner,
            string args,
            ITestOutputHelper testOutputHelper,
            IEnumerable<object> dependencies = null) where T : class
        {
            return runner.RunInMem(args.SplitArgs(), testOutputHelper, dependencies);
        }

        public static AppRunnerResult RunInMem<T>(
            this AppRunner<T> runner, 
            string[] args, 
            ITestOutputHelper testOutputHelper,
            IEnumerable<object> dependencies = null) where T : class
        {
            var consoleOut = new TestConsoleWriter(testOutputHelper);
            runner.OverrideConsoleOut(consoleOut);
            runner.OverrideConsoleError(consoleOut);

            var resolver = new TestDependencyResolver();
            resolver.Register(new TestWriter(consoleOut));
            foreach (var dependency in dependencies ?? Enumerable.Empty<object>())
            {
                resolver.Register(dependency);
            }
            runner.UseDependencyResolver(resolver);

            var inputs = new TestOutputs();
            resolver.Register(inputs);

            var exitCode = runner.Run(args);
            return new AppRunnerResult(exitCode, consoleOut.ToString(), inputs);
        }
    }
}