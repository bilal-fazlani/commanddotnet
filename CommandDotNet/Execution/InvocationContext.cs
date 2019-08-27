namespace CommandDotNet.Execution
{
    public class InvocationContext
    {
        public object Instance { get; set; }
        public IInvocation InterceptorInvocation { get; set; }
        public IInvocation CommandInvocation { get; set; }
    }
}