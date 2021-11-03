using CommandDotNet.Execution;

namespace CommandDotNet.Spectre
{
    public static class MiddlewareSteps
    {
        /// <summary>Inserts shortly after DebugDirective which is the first middleware in the pipeline</summary>
        public static MiddlewareStep Spectre { get; set; } = Execution.MiddlewareSteps.DebugDirective + 100;
    }
}