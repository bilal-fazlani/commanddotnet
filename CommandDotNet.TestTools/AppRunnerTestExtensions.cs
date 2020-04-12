using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Diagnostics.Parse;
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
                .RunInMem(args, logger, config: TestConfig.Silent);
            return state;
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string args,
            Action<string> logLine = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null,
            TestConfig config = null)
        {
            return runner.RunInMem(args.SplitArgs(), logLine, onReadLine, pipedInput, promptResponder, config);
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string[] args,
            Action<string> logLine = null,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null,
            TestConfig config = null)
        {
            logLine = logLine ?? Console.WriteLine;
            config = config ?? TestConfig.Default;

            IDisposable logProvider = config.PrintCommandDotNetLogs
                ? TestToolsLogProvider.InitLogProvider(logLine)
                : new DisposableAction(() => { });

            using (logProvider)
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

                try
                {
                    var exitCode = runner.Run(args);
                    return new AppRunnerResult(exitCode, runner, context, testConsole, captures, config)
                        .LogResult(logLine);
                }
                catch (Exception e)
                {
                    if (!config.Source.IsNullOrWhitespace())
                    {
                        logLine("");
                        logLine($"TestConfig source:{config.Source}");
                    }

                    var result = new AppRunnerResult(1, runner, context, testConsole, captures, config, e);
                    if (config.OnError.CaptureAndReturnResult)
                    {
                        testConsole.Error.WriteLine(e.Message);
                        logLine(e.Message);
                        logLine(e.StackTrace);
                        return result.LogResult(logLine, onError: true);
                    }

                    result.LogResult(logLine, onError: true);
                    throw;
                }
            }
        }

        internal static AppRunnerResult LogResult(this AppRunnerResult result, Action<string> logLine, bool onError = false)
        {
            var print = onError || result.EscapedException != null 
                ? result.Config.OnError.Print 
                : result.Config.OnSuccess.Print;

            if (print.ConsoleOutput)
            {
                var consoleAll = result.ConsoleAll;
                if (consoleAll.EndsWith(Environment.NewLine))
                {
                    // logLine adds a NewLine and if the console output ends with NewLine
                    // then an extra NewLine will appear in the output which will be misleading.
                    consoleAll = consoleAll.Substring(0, consoleAll.Length - NewLine.Length);
                }
                logLine($"{NewLine}Console output <begin> ------------------------------");
                logLine(consoleAll.IsNullOrWhitespace() ? "<no output>" : consoleAll);
                logLine($"Console output <end> ------------------------------{NewLine}");
            }

            var context = result.CommandContext;
            if (print.CommandContext)
            {
                logLine("");
                logLine(context?.ToString(new Indent(), includeOriginalArgs: true));
            }

            if (print.ParseReport && context?.ParseResult != null)
            {
                logLine("");
                logLine($"{NewLine}Parse report <begin> ------------------------------");
                ParseReporter.Report(context, logLine);
                logLine($"Parse report <end> ------------------------------{NewLine}");
            }

            if (print.AppConfig)
            {
                logLine("");
                logLine(result.Runner.ToString());
            }

            return result;
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