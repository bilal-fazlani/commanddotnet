using System;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;

namespace CommandDotNet.Help
{
    internal static class HelpMiddleware
    {
        internal static AppRunner UseHelpMiddleware(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += AddHelpOption;
                c.UseMiddleware(DisplayHelpIfSpecified, MiddlewareStages.PostParseInputPreBindValues);

                // TODO: consider adding another middleware to check CommandContext.ShowHelpForCommand (of type Command)
                //       and if set, show help. Removes the tight coupling on this Print method
            });
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