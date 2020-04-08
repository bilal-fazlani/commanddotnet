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
                c.UseMiddleware(CheckIfShouldShowHelp,
                    MiddlewareSteps.Help.CheckIfShouldShowHelp.Stage,
                    MiddlewareSteps.Help.CheckIfShouldShowHelp.Order);
                c.UseMiddleware(PrintHelp,
                    MiddlewareSteps.Help.PrintHelp.Stage,
                    MiddlewareSteps.Help.PrintHelp.Order);
            });
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (args.CommandBuilder.Command.ContainsArgumentNode(Constants.HelpOptionName))
            {
                return;
            }

            var appSettingsHelp = args.CommandContext.AppConfig.AppSettings.Help;

            var option = new Option(Constants.HelpOptionName, 'h',
                TypeInfo.Flag, ArgumentArity.Zero,
                aliases: new[] { "?" },
                definitionSource: typeof(HelpMiddleware).FullName)
            {
                Description = "Show help information",
                IsMiddlewareOption = true,
                ShowInHelp = appSettingsHelp.PrintHelpOption
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> CheckIfShouldShowHelp(CommandContext ctx, ExecutionDelegate next)
        {
            var parseResult = ctx.ParseResult;
            var targetCommand = parseResult.TargetCommand;

            if (parseResult.ParseError != null)
            {
                var console = ctx.Console;
                console.Error.WriteLine(parseResult.ParseError.Message);
                console.Error.WriteLine();
                ctx.ShowHelpOnExit = true;
                return Task.FromResult(1);
            }

            if (parseResult.HelpWasRequested())
            {
                ctx.ShowHelpOnExit = true;
                return Task.FromResult(0);
            }

            if (!targetCommand.IsExecutable)
            {
                ctx.ShowHelpOnExit = true;
                return Task.FromResult(0);
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