using System.IO;

namespace CommandDotNet.Tests
{
    public static class AppSettingsTestExtensions
    {
        public static AppRunnerResult RunInMem<T>(this AppRunner<T> runner, params string[] args) where T : class
        {
            var consoleOut = new StringWriter();
            runner.OverrideConsoleOut(consoleOut);
            runner.OverrideConsoleError(consoleOut);
            var exitCode = runner.Run(args);
            return new AppRunnerResult(exitCode, consoleOut.ToString());
        }
    }
}