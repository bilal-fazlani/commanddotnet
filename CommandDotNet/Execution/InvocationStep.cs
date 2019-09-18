namespace CommandDotNet.Execution
{
    public class InvocationStep
    {
        /// <summary>The instance of the class containing the invocation method</summary>
        public object Instance { get; set; }

        /// <summary>The command the invocation will fulfill</summary>
        public Command Command { get; set; }

        /// <summary>The invocation </summary>
        public IInvocation Invocation { get; set; }
    }
}