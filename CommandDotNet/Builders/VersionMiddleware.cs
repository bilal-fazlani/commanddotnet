using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    internal static class VersionMiddleware
    {
        private const string VersionOptionName = "version";
        private const string VersionTemplate = "-v | --" + VersionOptionName;
        
        internal static ExecutionBuilder UseVersionMiddleware(this ExecutionBuilder builder, int orderWithinParsingStage)
        {
            builder.BuildEvents.OnCommandCreated += AddVersionOption;
            builder.AddMiddlewareInStage(DisplayVersionIfSpecified, MiddlewareStages.Parsing, orderWithinParsingStage);

            return builder;
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

        private static Task<int> DisplayVersionIfSpecified(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            if (commandContext.ParseResult.ArgumentValues.Contains(VersionOptionName))
            {
                Print(commandContext.AppSettings);
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        private static void Print(AppSettings appSettings)
        {
            if (!appSettings.EnableVersionOption)
            {
                return;
            }

            var hostAssembly = Assembly.GetEntryAssembly();
            if (hostAssembly == null)
            {
                throw new AppRunnerException(
                    "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                    "This is a known issue when running unit tests in .net framework. " +
                    "Consider disabling for test runs. " +
                    "https://tinyurl.com/y6rnjqsg");
            }

            var filename = Path.GetFileName(hostAssembly.Location);
            appSettings.Console.Out.WriteLine(filename);

            var fvi = FileVersionInfo.GetVersionInfo(hostAssembly.Location);
            appSettings.Console.Out.WriteLine(fvi.ProductVersion);
        }
    }
}