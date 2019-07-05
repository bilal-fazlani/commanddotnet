namespace CommandDotNet.Builders
{
    internal interface ICommandBuilder
    {
        ICommand Command { get; }
        void AddArgument(IArgument argument);
    }
}