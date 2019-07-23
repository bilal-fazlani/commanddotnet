namespace CommandDotNet.Builders
{
    public interface ICommandBuilder
    {
        Command Command { get; }
        void AddSubCommand(Command command);
        void AddArgument(IArgument argument);
    }
}