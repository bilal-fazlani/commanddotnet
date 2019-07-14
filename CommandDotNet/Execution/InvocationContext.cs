namespace CommandDotNet.Execution
{
    public class InvocationContext
    {
        public object Instance { get; set; }
        public IInvocation InstantiateInvocation { get; set; }
        public IInvocation CommandInvocation { get; set; }
    }
}