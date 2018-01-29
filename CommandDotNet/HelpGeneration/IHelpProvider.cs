using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.HelpGeneration
{
    public interface IHelpProvider
    {
        string GetHelpText(ICommand app);
    }
}