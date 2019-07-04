using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet
{
    internal interface IOptionSource
    {
        void AddOption(ICommandBuilder commandBuilder);
    }
}