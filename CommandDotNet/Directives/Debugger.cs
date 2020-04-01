using System;
using System.Threading;
using CommandDotNet.Rendering;

namespace CommandDotNet.Directives
{
    [Obsolete("Moved to namespace Diagnostics")]
    public static class Debugger
    {
        [Obsolete("use Diagnostics.Debugger.AttachIfDebugDirective instead")]
        public static void AttachIfDebugDirective(string[] args, 
            CancellationToken? cancellationToken = null, 
            IConsole console = null)
        {
            Diagnostics.Debugger.AttachIfDebugDirective(args, cancellationToken, console);
        }

        [Obsolete("use Diagnostics.Debugger.Attach instead")]
        public static void Attach(
            CancellationToken cancellationToken, 
            IConsole console,
            bool waitForDebuggerToAttach)
        {
            Diagnostics.Debugger.Attach(cancellationToken, console, waitForDebuggerToAttach);
        }
    }
}