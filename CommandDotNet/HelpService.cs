using CommandDotNet.HelpGeneration;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class HelpService
    {
        public static void Print(AppSettings appSettings, ICommand command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}