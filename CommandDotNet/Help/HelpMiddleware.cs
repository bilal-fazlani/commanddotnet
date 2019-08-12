using System;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling.Definitions;
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

        private static Task<int> DisplayHelp(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var parseResult = commandContext.ParseResult;
            if (parseResult.ParseError != null)
            {
                var console = commandContext.Console;
                console.Error.WriteLine(parseResult.ParseError.Message);
                console.Error.WriteLine();
                Print(commandContext, parseResult.TargetCommand);
                return Task.FromResult(1);
            }

            if (parseResult.ArgumentValues.Contains(Constants.HelpArgumentTemplate.LongName))
            {
                Print(commandContext, parseResult.TargetCommand);
                return Task.FromResult(0);
            }

            var commandDef = parseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null && !commandDef.IsExecutable)
            {
                Print(commandContext, commandDef.Command);
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