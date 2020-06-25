using CommandDotNet.Execution;

namespace CommandDotNet.ReadLineRepl
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep ReplSession { get; } = CommandDotNet.Execution.MiddlewareSteps.Help.CheckIfShouldShowHelp - 1000;
    }
}