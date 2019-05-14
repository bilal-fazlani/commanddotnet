using System;

namespace CommandDotNet.Tests
{
    public class AppRunnerResult
    {
        public int ExitCode { get; }
        public string ConsoleOut { get; }

        public AppRunnerResult(int exitCode, string consoleOut)
        {
            this.ExitCode = exitCode;
            this.ConsoleOut = consoleOut;
        }

        public string[] GetConsoleOutLines() => this.ConsoleOut.Split(Environment.NewLine);
    }
}