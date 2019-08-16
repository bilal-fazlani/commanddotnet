using System;

namespace CommandDotNet.TestTools
{
    /// <summary>
    /// Used to write test & console output to a test framework.<br/>
    /// i.e. if using XUnit, new Logger(testOutputHelper.WriteLine);
    /// </summary>
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