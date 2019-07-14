using System;
using CommandDotNet.Execution;
using CommandDotNet.Help;

namespace CommandDotNet.Builders
{
    internal static class HelpMiddleware
    {
        internal static ExecutionBuilder UseHelpMiddleware(this ExecutionBuilder builder, int orderWithinParsingStage)
        {
            builder.BuildEvents.OnCommandCreated += AddHelpOption;
            builder.AddMiddlewareInStage(DisplayHelpIfSpecified, MiddlewareStages.Parsing, orderWithinParsingStage);

            return builder;
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
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

            args.CommandBuilder.AddArgument(option);
        }

        private static int DisplayHelpIfSpecified(CommandContext commandContext, Func<CommandContext, int> next)
        {
            if (commandContext.ParseResult.ArgumentValues.Contains(Constants.HelpArgumentTemplate.Name))
            {
                Print(commandContext.AppSettings, commandContext.ParseResult.Command);
                return 0;
            }

            return next(commandContext);
        }

        public static void Print(AppSettings appSettings, ICommand command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Console.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}