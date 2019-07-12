using System;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Help;

namespace CommandDotNet.Builders
{
    internal class HelpOptionSource : IOptionSource
    {
        public void AddOption(ICommandBuilder commandBuilder)
        {
            var option = new Option(Constants.HelpTemplate, ArgumentArity.Zero)
            {
                Description = "Show help information",
                TypeInfo = new TypeInfo
                {
                    Type = typeof(bool),
                    UnderlyingType = typeof(bool),
                    DisplayName = Constants.TypeDisplayNames.Flag
                },
                IsSystemOption = true,
                Arity = ArgumentArity.Zero
            };

            commandBuilder.AddArgument(option);
        }
        
        internal static int HelpMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            if (commandContext.ParseResult.ValuesByArgument.Any(v => v.Key.Name == Constants.HelpArgumentTemplate.Name))
            {
                Print(commandContext.AppSettings, commandContext.ParseResult.Command);
                return 0;
            }

            return next(commandContext);
        }

        public static void Print(AppSettings appSettings, ICommand command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}