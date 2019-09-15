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
                c.UseMiddleware(DisplayHelp, MiddlewareStages.PostParseInputPreBindValues);
            });
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (args.CommandBuilder.Command.FindOption(Constants.HelpArgumentTemplate.LongName) != null)
            {
                return;
            }

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

        private static Task<int> DisplayHelp(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseResult = commandContext.ParseResult;
            var targetCommand = parseResult.TargetCommand;

            if (parseResult.ParseError != null)
            {
                var console = commandContext.Console;
                console.Error.WriteLine(parseResult.ParseError.Message);
                console.Error.WriteLine();
                Print(commandContext, targetCommand);
                return Task.FromResult(1);
            }

            if (parseResult.ArgumentValues.Contains(Constants.HelpArgumentTemplate.LongName))
            {
                Print(commandContext, targetCommand);
                return Task.FromResult(0);
            }

            if (!targetCommand.IsExecutable)
            {
                Print(commandContext, targetCommand);
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        private static void Print(CommandContext commandContext, Command command)
        {
            var helpText = commandContext.AppConfig.HelpProvider.GetHelpText(command);
            commandContext.Console.Out.WriteLine(helpText);
        }
    }
}