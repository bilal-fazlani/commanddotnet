using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    TypeDisplayName = Constants.TypeDisplayNames.Flag,
                    IsSystemOption = true,
                    Arity = ArgumentArity.Zero
                };

                commandBuilder.AddArgument(option);
            }
        }

        internal static int VersionMiddleware(ExecutionContext executionContext, Func<ExecutionContext, int> next)
        {
            if (executionContext.ParseResult.Values.Any(v => v.Argument.Name == VersionOptionName))
            {
                Print(executionContext.AppSettings);
                return 0;
            }

            return next(executionContext);
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
            appSettings.Out.WriteLine(filename);

            var fvi = FileVersionInfo.GetVersionInfo(hostAssembly.Location);
            appSettings.Out.WriteLine(fvi.ProductVersion);
        }
    }
}