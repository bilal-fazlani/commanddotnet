using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.HelpGeneration
{
    public interface IHelpGenerator
    {
        string GetHelpText(ICommand app);
    }
}