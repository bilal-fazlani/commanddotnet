using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
    public static class AppRunnerTestExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner, 
            string[] args,
            ILogger logger,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            var testConsole = new TestConsole(
                onReadLine,
                pipedInput == null
                    ? (Func<TestConsole, string>) null
                    : console => pipedInput?.ToCsv(Environment.NewLine));
                    
            runner.Configure(c => c.Console = testConsole);
            var outputs = InjectTestOutputs(runner);

            var exitCode = runner.Run(args);
            var consoleOut = testConsole.Joined.ToString();
            
            // output to console to help debugging failed tests
            logger?.WriteLine(consoleOut);

            return new AppRunnerResult(exitCode, testConsole, outputs);
        }

        private static TestOutputs InjectTestOutputs(AppRunner runner)
        {
            TestOutputs outputs = new TestOutputs();
            runner.Configure(c => c.UseMiddleware((context, next) =>
            {
                context.InvocationContexts.All
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