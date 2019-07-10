using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Directives
{
    internal static class DebugDirective
    {
        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have settled
        // on a better design for implementing the control flow
        // i.e. middleware pipeline
        internal static bool InTestHarness { private get; set; }

        // adapted from https://github.com/dotnet/command-line-api directives
        public static int DebugMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            if (commandContext.Tokens.TryGetDirective("debug", out string value))
            {
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                commandContext.AppSettings.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!InTestHarness && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }

            return next(commandContext);
        }
    }
}