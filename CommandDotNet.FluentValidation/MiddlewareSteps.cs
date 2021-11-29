using CommandDotNet.Execution;

namespace CommandDotNet.FluentValidation
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep FluentValidation { get; set; } = Execution.MiddlewareSteps.ValidateArity-1000;
    }
}