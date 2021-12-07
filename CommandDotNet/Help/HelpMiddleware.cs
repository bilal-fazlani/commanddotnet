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
                c.UseMiddleware(CheckIfShouldShowHelp, MiddlewareSteps.Help.CheckIfShouldShowHelp);
                c.UseMiddleware(PrintHelp, MiddlewareSteps.Help.PrintHelpOnExit);
                c.BuildEvents.OnCommandCreated += AddHelpOption;
            });
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
        {
            var helpOptionName = Resources.A.Command_help;
            if (args.CommandBuilder.Command.ContainsArgumentNode(helpOptionName))
            {
                return;
            }

            var appSettingsHelp = args.CommandContext.AppConfig.AppSettings.Help;

            var option = new Option(helpOptionName, 'h',
                TypeInfo.Flag, ArgumentArity.Zero, BooleanMode.Implicit,
                aliases: new[] { "?" },
                definitionSource: typeof(HelpMiddleware).FullName)
            {
                Description = Resources.A.Command_help_description,
                IsMiddlewareOption = true,
                Hidden = !appSettingsHelp.PrintHelpOption
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> CheckIfShouldShowHelp(CommandContext ctx, ExecutionDelegate next)
        {
            var parseResult = ctx.ParseResult!;
            var targetCommand = parseResult.TargetCommand;

            if (parseResult.ParseError is { })
            {
                var console = ctx.Console;
                console.Error.WriteLine(parseResult.ParseError.Message);
                console.Error.WriteLine();
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Error;
            }

            if (parseResult.HelpWasRequested())
            {
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Success;
            }

            if (!targetCommand.IsExecutable)
            {
                var console = ctx.Console;
                console.Error.WriteLine(Resources.A.Parse_Required_command_was_not_provided);
                console.Error.WriteLine();
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Error;
            }

            return next(ctx);
        }

        private static Task<int> PrintHelp(CommandContext ctx, ExecutionDelegate next)
        {
            var result = next(ctx);

            if (ctx.ShowHelpOnExit)
            {
                ctx.PrintHelp();
            }

            return result;
        }
    }
}