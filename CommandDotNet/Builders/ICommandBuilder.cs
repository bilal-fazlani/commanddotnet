namespace CommandDotNet.Builders
{
    public interface ICommandBuilder
    {
        ICommand Command { get; }
        void AddSubCommand(ICommand command);
        void AddArgument(IArgument argument);
    }
}