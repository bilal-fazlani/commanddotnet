using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools.Prompts;

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
            ILogger logger,
            Func<CommandContext, T> capture,
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
            string[] args,
            ILogger logger,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null,
            bool returnResultOnError = false)
        {
            using (TestToolsLogProvider.InitLogProvider(logger))
            {
                var testConsole = new TestConsole(
                    onReadLine,
                    pipedInput,
                    promptResponder == null
                        ? (Func<TestConsole, ConsoleKeyInfo>) null
                        : promptResponder.OnReadKey);

                CommandContext context = null;
                runner.CaptureState(ctx => context = ctx, MiddlewareStages.PreTokenize);
                runner.Configure(c => c.Console = testConsole);
                var outputs = InjectTestOutputs(runner);

                void LogResult()
                {
                    logger.WriteLine("\nconsole output:\n");
                    logger.WriteLine(testConsole.Joined.ToString());
                }

                try
                {
                    var exitCode = runner.Run(args);
                    LogResult();
                    return new AppRunnerResult(exitCode, testConsole, outputs, context);
                }
                catch (Exception e)
                {
                    if (returnResultOnError)
                    {
                        testConsole.Error.WriteLine(e.Message);
                        logger.WriteLine(e.Message);
                        logger.WriteLine(e.StackTrace);
                        LogResult();
                        return new AppRunnerResult(1, testConsole, outputs, context);
                    }

                    LogResult();
                    throw;
                }
            }
        }

        private static TestOutputs InjectTestOutputs(AppRunner runner)
        {
            var outputs = new TestOutputs();
            runner.Configure(c =>
            {
                c.Services.Add(outputs);
                c.UseMiddleware(InjectTestOutputs, MiddlewareStages.PostBindValuesPreInvoke);
            });

            return outputs;
        }

        private static Task<int> InjectTestOutputs(CommandContext commandContext, ExecutionDelegate next)
        {
            var outputs = commandContext.AppConfig.Services.Get<TestOutputs>();
            commandContext.InvocationPipeline.All
                .Select(i => i.Instance)
                .ForEach(instance =>
                {
                    instance.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(p => p.PropertyType == typeof(TestOutputs))
                        .ForEach(p =>
                        {
                            // principal of least surprise
                            // if the test class sets the instance, then use that instance
                            var value = (TestOutputs)p.GetValue(instance);
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