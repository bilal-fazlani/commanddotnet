using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;

namespace CommandDotNet.Builders
{
    internal static class VersionMiddleware
    {
        internal static readonly string VersionOptionName = Constants.VersionOptionName;

        internal static AppRunner UseVersionMiddleware(AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += AddVersionOption;
                c.UseMiddleware(DisplayVersionIfSpecified, MiddlewareSteps.Version.Stage, MiddlewareSteps.Version.Order);
            });
        }

        private static void AddVersionOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (!args.CommandBuilder.Command.IsRootCommand())
            {
                return;
            }

            if (args.CommandBuilder.Command.ContainsArgumentNode(VersionOptionName))
            {
                return;
            }

            var option = new Option(VersionOptionName, 'v', 
                TypeInfo.Flag, ArgumentArity.Zero, 
                definitionSource: typeof(VersionMiddleware).FullName)
            {
                Description = "Show version information",
                IsMiddlewareOption = true
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> DisplayVersionIfSpecified(CommandContext commandContext,
            ExecutionDelegate next)
        {
            if (commandContext.RootCommand.HasInputValues(VersionOptionName))
            {
                Print(commandContext, commandContext.Console);
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        private static void Print(CommandContext commandContext, IConsole console)
        {
            var appInfo = AppInfo.GetAppInfo(commandContext);

            console.Out.WriteLine(appInfo.FileName);
            console.Out.WriteLine(appInfo.Version);
        }
    }
}