using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CommandDotNet.Directives
{
    internal class DebugDirective
    {
        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have settled
        // on a better design for implementing the control flow
        // i.e. middleware pipeline
        internal static bool InTestHarness { private get; set; }

        // adapted from https://github.com/dotnet/command-line-api directives
        public static int Execute(ExecutionResult executionResult, Func<ExecutionResult, int> next)
        {
            if (executionResult.Tokens.TryGetDirective("debug", out string value))
            {
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                executionResult.AppSettings.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!InTestHarness && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }

            return next(executionResult);
        }
    }
}