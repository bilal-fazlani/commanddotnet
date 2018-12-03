namespace CommandDotNet.CommandInterceptor
{
    public interface ICommandInvoker
    {
        object Invoke(CommandInvocation commandInvocation);
    }
}
