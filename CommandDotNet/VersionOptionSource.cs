using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class VersionOptionSource : IOptionSource
    {
        public const string VersionTemplate = "-v | --version";

        private readonly AppSettings _appSettings;

        public VersionOptionSource(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void AddOption(ICommandBuilder commandBuilder)
        {
            if (_appSettings.EnableVersionOption && commandBuilder.IsRootCommand())
            {
                var option = new CommandOption(VersionTemplate, ArgumentArity.Zero)
                {
                    Description = "Show version information",
                    TypeDisplayName = Constants.TypeDisplayNames.Flag,
                    IsSystemOption = true,
                    InvokeAsCommand = () => Print(_appSettings)
                };

                commandBuilder.AddArgument(option);
            }
        }
        
        public static void Print(AppSettings appSettings)
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