using System.Diagnostics;
using System.Threading.Tasks;
using CommandDotNet.Rendering;

namespace CommandDotNet.Directives
{
    public static class Debugger
    {
        public static void Attach(IConsole console, bool waitForDebuggerToAttach)
        {
            var process = Process.GetCurrentProcess();
            console.Out.WriteLine($"Attach your debugger to process {process.Id} ({process.ProcessName}).");

            while (waitForDebuggerToAttach && !System.Diagnostics.Debugger.IsAttached)
            {
                Task.Delay(500);
            }
        }
    }
}