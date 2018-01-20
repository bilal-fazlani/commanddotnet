using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.HelpGeneration
{
    public interface IHelpGenerator
    {
        string GetHelpText(CommandLineApplication app);
    }
}