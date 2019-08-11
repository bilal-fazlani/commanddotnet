namespace CommandDotNet.Execution
{
    public class InvocationContext
    {
        public object Instance { get; set; }
        public IInvocation CommandMiddlewareInvocation { get; set; }
        public IInvocation CommandInvocation { get; set; }
    }
}