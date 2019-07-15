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

        internal static AppBuilder UseDebugDirective(this AppBuilder appBuilder)
        {
            appBuilder.AddMiddlewareInStage(AttachDebugger, MiddlewareStages.PreTransformInput, int.MinValue);

            return appBuilder;
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        public static Task<int> AttachDebugger(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            if (commandContext.Tokens.TryGetDirective("debug", out string value))
            {
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                commandContext.Console.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!InTestHarness && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }

            return next(commandContext);
        }
    }
}