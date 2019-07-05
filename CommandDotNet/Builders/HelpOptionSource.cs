using System;
using CommandDotNet.Help;

namespace CommandDotNet.Builders
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
            var option = new Option(Constants.HelpTemplate, ArgumentArity.Zero)
            {
                Description = "Show help information",
                TypeDisplayName = Constants.TypeDisplayNames.Flag,
                IsSystemOption = true,
                InvokeAsCommand = () => Print(_appSettings, commandBuilder.Command)
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