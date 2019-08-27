using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.Rendering;

namespace CommandDotNet.Directives
{
    public static class Debugger
    {
        public static void Attach(
            CancellationToken cancellationToken, 
            IConsole console,
            bool waitForDebuggerToAttach)
        {
            var process = Process.GetCurrentProcess();
            console.Out.WriteLine($"Attach your debugger to process {process.Id} ({process.ProcessName}).");

            if (!waitForDebuggerToAttach)
            {
                // Don't wait. Could be within a test.
                // Could also be a long running app and this is a courtesy message.
                return;
            }

            while (!System.Diagnostics.Debugger.IsAttached 
                   && !cancellationToken.IsCancellationRequested)
            {
                Task.Delay(500, cancellationToken);
            }
        }
    }
}