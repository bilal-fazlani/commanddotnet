namespace CommandDotNet.CommandInvoker
{
    public interface ICommandInvoker
    {
        object Invoke(CommandInvocation commandInvocation);
    }
}
