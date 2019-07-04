namespace CommandDotNet.MicrosoftCommandLineUtils
{
    internal interface ICommandBuilder : ICommand
    {
        void AddArgument(IArgument argument);
    }
}