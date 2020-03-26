using System;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.TestTools
{
    public class TestToolsLogProvider : ILogProvider
    {
        private ILogger _logger;

        public static IDisposable InitLogProvider(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (LogProvider.CurrentLogProvider is TestToolsLogProvider provider)
            {
                provider._logger = logger;
            }
            else
            {
                provider = new TestToolsLogProvider { _logger = logger };
                LogProvider.SetCurrentLogProvider(provider);
            }

            LogProvider.IsDisabled = false;
            return new DisposableAction(() => LogProvider.IsDisabled = true);
        }

        public Logging.Logger GetLogger(string name)
        {
            return (level, messageFunc, exception, parameters) =>
            {
                var msg = messageFunc?.Invoke();

                if (msg != null && (parameters?.Any() ?? false))
                {
                    msg = string.Format(msg, parameters);
                }

                if (msg != null || exception != null)
                {
                    _logger?.WriteLine($"{level.ToString().First()} {name} > {msg} {exception}");
                }

                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            return new NullDisposable();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            return new NullDisposable();
        }

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}