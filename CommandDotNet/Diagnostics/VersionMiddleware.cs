using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;

namespace CommandDotNet.Diagnostics
{
    internal static class VersionMiddleware
    {
        private static string VersionOptionName => Resources.A.Command_version;

        internal static AppRunner UseVersionMiddleware(AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(DisplayVersionIfSpecified, MiddlewareSteps.Version);
                c.BuildEvents.OnCommandCreated += AddVersionOption;
            });
        }

        private static void AddVersionOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (!args.CommandBuilder.Command.IsRootCommand())
            {
                return;
            }

            if (args.CommandBuilder.Command.ContainsArgumentNode(Resources.A.Command_version))
            {
                return;
            }

            var option = new Option(VersionOptionName, 'v', 
                TypeInfo.Flag, ArgumentArity.Zero, BooleanMode.Implicit,
                definitionSource: typeof(VersionMiddleware).FullName)
            {
                Description = Resources.A.Command_version_description,
                IsMiddlewareOption = true
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> DisplayVersionIfSpecified(CommandContext commandContext,
            ExecutionDelegate next)
        {
            if (commandContext.RootCommand!.HasInputValues(VersionOptionName))
            {
                Print(commandContext.Console);
                return ExitCodes.Success;
            }

            return next(commandContext);
        }

        private static void Print(IConsole console)
        {
            var appInfo = AppInfo.Instance;

            console.Out.WriteLine(appInfo.FileName);
            console.Out.WriteLine(appInfo.Version);
        }
    }
}