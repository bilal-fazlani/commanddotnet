using System;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;

namespace CommandDotNet.Help
{
    internal static class HelpMiddleware
    {
        internal static AppBuilder UseHelpMiddleware(this AppBuilder builder)
        {
            builder.BuildEvents.OnCommandCreated += AddHelpOption;
            builder.AddMiddlewareInStage(DisplayHelpIfSpecified, MiddlewareStages.PostParseInputPreBindValues);

            return builder;
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
        {
            var option = new Option(Constants.HelpTemplate, ArgumentArity.Zero, aliases: new []{"?"})
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

        private static Task<int> DisplayHelpIfSpecified(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            if (commandContext.ParseResult.ArgumentValues.Contains(Constants.HelpArgumentTemplate.LongName))
            {
                Print(commandContext.AppSettings, commandContext.ParseResult.TargetCommand);
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        public static void Print(AppSettings appSettings, Command command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Console.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}