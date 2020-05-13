using CommandDotNet.Execution;

namespace CommandDotNet.DataAnnotations
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep DataAnnotations { get; set; } = new MiddlewareStep(MiddlewareStages.PostBindValuesPreInvoke);
    }
}