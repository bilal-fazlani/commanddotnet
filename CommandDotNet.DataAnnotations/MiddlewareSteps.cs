using CommandDotNet.Execution;

namespace CommandDotNet.DataAnnotations
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep DataAnnotations { get; set; } = Execution.MiddlewareSteps.ValidateArity - 1100;
    }
}