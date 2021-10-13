using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;

namespace CommandDotNet.Builders
{
    internal static class VersionMiddleware
    {
        private static string VersionOptionName => HelpText.Instance.version_lc;

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

            if (args.CommandBuilder.Command.ContainsArgumentNode(HelpText.Instance.version_lc))
            {
                return;
            }

            var option = new Option(VersionOptionName, 'v', 
                TypeInfo.Flag, ArgumentArity.Zero, 
                definitionSource: typeof(VersionMiddleware).FullName)
            {
                Description = HelpText.Instance.Show_version_information,
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