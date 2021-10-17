﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Diagnostics
{
    public static class Debugger
    {
        /// <summary>Attaches a debugger if the [debug] directive exists</summary>
        public static void AttachIfDebugDirective(string[] args, 
            CancellationToken? cancellationToken = null, 
            IConsole? console = null)
        {
            if (args.HasDebugDirective())
            {
                Attach(
                    cancellationToken ?? CancellationToken.None, 
                    console ?? new SystemConsole(), 
                    true);
            }
        }

        /// <summary>Attaches a debugger</summary>
        public static void Attach(
            CancellationToken cancellationToken, 
            IConsole console,
            bool waitForDebuggerToAttach)
        {
            var process = Process.GetCurrentProcess();
            console.Out.WriteLine(Resources.A.Debugger_Attach_debugger(process.Id.ToString(), process.ProcessName));

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

        internal static bool HasDebugDirective(this TokenCollection tokens)
        {
            return tokens.TryGetDirective(Resources.A.Debugger_debug_lc, out _);
        }

        private static bool HasDebugDirective(this string[] args)
        {
            return args.Any(a => a.Equals($"[{Resources.A.Debugger_debug_lc}]", StringComparison.OrdinalIgnoreCase));
        }
    }
}