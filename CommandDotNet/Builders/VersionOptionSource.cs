using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    internal class VersionOptionSource : IOptionSource
    {
        private const string VersionOptionName = "version";
        private const string VersionTemplate = "-v | --" + VersionOptionName;

        private readonly AppSettings _appSettings;

        public VersionOptionSource(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void AddOption(ICommandBuilder commandBuilder)
        {
            if (_appSettings.EnableVersionOption && commandBuilder.Command.IsRootCommand())
            {
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

                commandBuilder.AddArgument(option);
            }
        }

        internal static int VersionMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            if (commandContext.ParseResult.ArgumentValues.Contains(VersionOptionName))
            {
                Print(commandContext.AppSettings);
                return 0;
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