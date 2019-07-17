using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Directives
{
    internal static class DebugDirective
    {
        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have a way
        // to call UseDebugDirective from the tests.
        internal static bool InTestHarness { private get; set; }

        internal static AppBuilder UseDebugDirective(this AppBuilder appBuilder, bool? waitForDebuggerToAttach = null)
        {
            appBuilder.ContextData.Add(new DebugDirectiveContext(waitForDebuggerToAttach ?? !InTestHarness));
            appBuilder.AddMiddlewareInStage(AttachDebugger, MiddlewareStages.PreTransformInput, int.MinValue);

            return appBuilder;
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> AttachDebugger(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            if (commandContext.Tokens.TryGetDirective("debug", out _))
            {
                var waitForDebuggerToAttach = commandContext.ExecutionConfig.ContextData.Get<DebugDirectiveContext>().WaitForDebuggerToAttach;
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                commandContext.Console.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (waitForDebuggerToAttach && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }

            return next(commandContext);
        }

        private class DebugDirectiveContext
        {
            public bool WaitForDebuggerToAttach { get; }

            public DebugDirectiveContext(bool waitForDebuggerToAttach)
            {
                WaitForDebuggerToAttach = waitForDebuggerToAttach;
            }
        }
    }
}