using System;

namespace CommandDotNet.Tests.Utils
{
    public class Logger : ILogger
    {
        private readonly Action<string> _logLine;

        public Logger(Action<string> logLine)
        {
            _logLine = logLine ?? throw new ArgumentNullException(nameof(logLine));
        }

        public void WriteLine(string log) => _logLine(log);
    }
}