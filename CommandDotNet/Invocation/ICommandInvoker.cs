namespace CommandDotNet.Invocation
{
    public interface ICommandInvoker
    {
        object Invoke(CommandInvocation commandInvocation);
    }
}
