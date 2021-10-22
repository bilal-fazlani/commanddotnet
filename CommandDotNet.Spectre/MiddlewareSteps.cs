using CommandDotNet.Execution;

namespace CommandDotNet.Spectre
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep Spectre { get; set; } = new MiddlewareStep(MiddlewareStages.PreTokenize);
    }
}