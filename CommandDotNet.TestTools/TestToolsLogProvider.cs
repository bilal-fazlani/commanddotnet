using System;
using System.Linq;
using CommandDotNet.Logging;

namespace CommandDotNet.TestTools
{
    internal class TestToolsLogProvider : ILogProvider
    {
        private ILogger _logger;

        internal static void InitLogProvider(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (!(LogProvider.CurrentLogProvider is TestToolsLogProvider provider))
            {
                provider = new TestToolsLogProvider();
                LogProvider.SetCurrentLogProvider(provider);
            }

            provider._logger = logger;
        }

        public Logging.Logger GetLogger(string name)
        {
            return (level, messageFunc, exception, parameters) =>
            {
                var msg = messageFunc?.Invoke();

                if (parameters?.Any() ?? false)
                {
                    msg = string.Format(msg, parameters);
                }

                _logger?.WriteLine($"{name}: {level.ToString().First()} {msg} {exception}");

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