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
                    ? Task.FromResult<int>(0) 
                    : next(context);
            }, middlewareStage, orderWithinStage));
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string[] args,
            ILogger logger,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null,
            IPromptResponder promptResponder = null)
        {
            TestToolsLogProvider.InitLogProvider(logger);

            var testConsole = new TestConsole(
                onReadLine,
                pipedInput,
                promptResponder == null
                    ? (Func<TestConsole, ConsoleKeyInfo>)null
                    : promptResponder.OnReadKey);
                    
            runner.Configure(c => c.Console = testConsole);
            var outputs = InjectTestOutputs(runner);

            try
            {
                var exitCode = runner.Run(args);
                var consoleOut = testConsole.Joined.ToString();

                logger?.WriteLine("\nconsole output:\n");
                logger?.WriteLine(consoleOut);
                return new AppRunnerResult(exitCode, testConsole, outputs);
            }
            catch (Exception e)
            {
                logger?.WriteLine("\nconsole output:\n");
                logger?.WriteLine(testConsole.Joined.ToString());
                throw;
            }
        }

        private static TestOutputs InjectTestOutputs(AppRunner runner)
        {
            TestOutputs outputs = new TestOutputs();
            runner.Configure(c => c.UseMiddleware((context, next) =>
            {
                context.InvocationPipeline.All
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
                                var value = (TestOutputs) p.GetValue(instance);
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
                return next(context);
            }, MiddlewareStages.PostBindValuesPreInvoke));

            return outputs;
        }
    }
}