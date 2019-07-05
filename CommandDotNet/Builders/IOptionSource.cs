namespace CommandDotNet.Builders
{
    internal interface IOptionSource
    {
        void AddOption(ICommandBuilder commandBuilder);
    }
}