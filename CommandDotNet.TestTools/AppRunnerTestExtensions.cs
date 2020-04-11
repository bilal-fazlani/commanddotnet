using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools.Prompts;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
    public static class AppRunnerTestExtensions
    {
        /// <summary>
        /// Injects a middleware to capture state at specific point. <br/>
        /// This method does not prevent referenced objects from being mutated after capture. <br/>
        /// It is the responsibility of capture action to clone data to prevent mutation.
        /// </summary>
        public static AppRunner CaptureState(this AppRunner runner, Action<CommandContext> capture,
            MiddlewareStages middlewareStage, int? orderWithinStage = null, bool exitAfterCapture = false)
        {
            return runner.Configure(b => b.UseMiddleware((context, next) =>
            {
                capture(context);
                return exitAfterCapture 
                    ? Task.FromResult(0) 
                    : next(context);
            }, middlewareStage, orderWithinStage));
        }

        /// <summary>
        /// Convenience wrapper for <see cref="CaptureState"/> to capture state from the <see cref="CommandContext"/>
        /// </summary>
        public static T GetFromContext<T>(this AppRunner runner,
            string[] args,
            Func<CommandContext, T> capture,
            Action<string> logger = null,
            MiddlewareStages middlewareStage = MiddlewareStages.PostBindValuesPreInvoke,
            int? orderWithinStage = null)
        {
            T state = default;
            runner.CaptureState(
                    ctx => state = capture(ctx),
                    exitAfterCapture: true,
                    middlewareStage: middlewareStage,
                    orderWithinStage: orderWithinStage)
                .RunInMem(args, logger);
            return state;
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string args,
            Action<string> logLine = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null,
            bool returnResultOnError = false)
        {
            return runner.RunInMem(args.SplitArgs(), logLine, onReadLine, pipedInput, promptResponder, returnResultOnError);
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string[] args,
            Action<string> logLine = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null,
            bool returnResultOnError = false)
        {
            logLine = logLine ?? (l => {});
            using (TestToolsLogProvider.InitLogProvider(logLine))
            {
                var testConsole = new TestConsole(
                    onReadLine,
                    pipedInput,
                    promptResponder == null
                        ? (Func<TestConsole, ConsoleKeyInfo>) null
                        : promptResponder.OnReadKey);
                runner.Configure(c => c.Console = testConsole);

                CommandContext context = null;
                runner.CaptureState(ctx => context = ctx, MiddlewareStages.PreTokenize);
                var captures = InjectTestCaptures(runner);

                void LogResult()
                {
                    var consoleAll = testConsole.All.ToString();
                    logLine($"{NewLine}Console output <begin> ------------------------------");
                    logLine(consoleAll.IsNullOrWhitespace() ? "<no output>" : consoleAll);
                    logLine($"Console output <end> ------------------------------{NewLine}");
                }

                try
                {
                    var exitCode = runner.Run(args);
                    LogResult();
                    return new AppRunnerResult(exitCode, testConsole, captures, context);
                }
                catch (Exception e)
                {
                    if (returnResultOnError)
                    {
                        testConsole.Error.WriteLine(e.Message);
                        logLine(e.Message);
                        logLine(e.StackTrace);
                        LogResult();
                        return new AppRunnerResult(1, testConsole, captures, context);
                    }

                    LogResult();
                    throw;
                }
            }
        }

        private static TestCaptures InjectTestCaptures(AppRunner runner)
        {
            var outputs = new TestCaptures();
            runner.Configure(c =>
            {
                c.Services.Add(outputs);
                c.UseMiddleware(InjectTestCaptures, MiddlewareStages.PostBindValuesPreInvoke);
            });

            return outputs;
        }

        private static Task<int> InjectTestCaptures(CommandContext commandContext, ExecutionDelegate next)
        {
            var outputs = commandContext.AppConfig.Services.Get<TestCaptures>();
            commandContext.InvocationPipeline.All
                .Select(i => i.Instance)
                .ForEach(instance =>
                {
                    instance.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(p => p.PropertyType == typeof(TestCaptures))
                        .ForEach(p =>
                        {
                            // principal of least surprise
                            // if the test class sets the instance, then use that instance
                            var value = (TestCaptures)p.GetValue(instance);
                            if (value == null)
                            {
                                p.SetValue(instance, outputs);
                            }
                            else
                            {
                                outputs.UseOutputsFromInstance(value);
                            }
                        });
                });
            return next(commandContext);
        }
    }
}