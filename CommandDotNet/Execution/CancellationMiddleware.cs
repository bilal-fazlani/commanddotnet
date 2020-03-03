using System;
using System.Threading;

namespace CommandDotNet.Execution
{
    internal static class CancellationMiddleware
    {
        // This code is not covered by automation.  I couldn't find a way to mimic and verify these events.
        // Tested manually using CancelMeApp in CommandDotNet.Examples

        internal static AppRunner UseCancellationHandlers(AppRunner appRunner)
        {
            var handlers = new Handlers();
            return appRunner.Configure(c =>
            {
                c.CancellationToken = handlers.CancellationToken;
                c.OnRunCompleted += _ => handlers.RemoveHandlers();
            });
        }

        private class Handlers
        {
            private readonly CancellationTokenSource _tokenSource;

            public CancellationToken CancellationToken => _tokenSource.Token;

            internal Handlers()
            {
                _tokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += Console_CancelKeyPress;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }

            internal void RemoveHandlers()
            {
                Console.CancelKeyPress -= Console_CancelKeyPress;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            }

            private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
            {
                Shutdown();
                e.Cancel = true;
            }

            private void CurrentDomain_ProcessExit(object sender, EventArgs e)
            {
                Shutdown();
            }

            private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                if (e.IsTerminating)
                {
                    Shutdown();
                }
            }

            private void Shutdown()
            {
                if (!_tokenSource.IsCancellationRequested)
                {
                    _tokenSource.Cancel();
                }
            }
        }
    }
}