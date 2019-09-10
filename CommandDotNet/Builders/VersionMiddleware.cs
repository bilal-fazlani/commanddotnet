using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;

namespace CommandDotNet.Builders
{
    internal static class VersionMiddleware
    {
        private const string VersionOptionName = "version";
        private const string VersionTemplate = "-v | --" + VersionOptionName;

        internal static AppRunner UseVersionMiddleware(AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += AddVersionOption;
                c.UseMiddleware(DisplayVersionIfSpecified, MiddlewareStages.PostParseInputPreBindValues);
            });
        }

        private static void AddVersionOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (!args.CommandBuilder.Command.IsRootCommand())
            {
                return;
            }

            var option = new Option(VersionTemplate, ArgumentArity.Zero)
            {
                Description = "Show version information",
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

        private static Task<int> DisplayVersionIfSpecified(CommandContext commandContext,
            ExecutionDelegate next)
        {
            if (commandContext.ParseResult.ArgumentValues.Contains(VersionOptionName))
            {
                Print(commandContext, commandContext.Console);
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        private static void Print(CommandContext commandContext, IConsole console)
        {
            var versionInfo = VersionInfo.GetVersionInfo(commandContext);

            console.Out.WriteLine(versionInfo.Filename);
            console.Out.WriteLine(versionInfo.Version);
        }
    }
}