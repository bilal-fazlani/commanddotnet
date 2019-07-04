using System;
using CommandDotNet.HelpGeneration;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class HelpOptionSource : IOptionSource
    {
        private readonly AppSettings _appSettings;

        public HelpOptionSource(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void AddOption(ICommandBuilder commandBuilder)
        {
            var option = new CommandOption(Constants.HelpTemplate, ArgumentArity.Zero)
            {
                Description = "Show help information",
                TypeDisplayName = Constants.TypeDisplayNames.Flag,
                IsSystemOption = true,
                InvokeAsCommand = () => Print(_appSettings, commandBuilder)
            };

            commandBuilder.AddArgument(option);
        }

        public static void Print(AppSettings appSettings, ICommand command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}