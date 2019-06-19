using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    public static class HelpBuilderExtensions
    {
        public static StringBuilder AppendUsageCommandNames(this StringBuilder sb, ICommand app, AppSettings appSettings)
        {
            var appName = GetAppName(app, appSettings.Help.UsageAppNameStyle);

            sb.Append(appName);
            foreach (var name in app.GetParentCommands(true).Reverse().Skip(1).Select(c => c.Name))
            {
                sb.Append(" ");
                sb.Append(name);
            }
            return sb;
        }

        public static string GetAppName(ICommand app, UsageAppNameStyle usageAppNameStyle)
        {
            switch (usageAppNameStyle)
            {
                case UsageAppNameStyle.Adaptive:
                    return GetAppNameAdaptive(app);
                case UsageAppNameStyle.DotNet:
                    return $"dotnet {GetAppFileName()}";
                case UsageAppNameStyle.GlobalTool:
                    var rootAppName = GetRootAppName(app);
                    if (rootAppName == null)
                    {
                        throw new AppRunnerException(
                            $"Invalid configuration: {nameof(ApplicationMetadataAttribute)}.{nameof(ApplicationMetadataAttribute.Name)} " +
                            $"is required for the root command when {nameof(UsageAppNameStyle)}.{nameof(UsageAppNameStyle.GlobalTool)} " +
                            $"is specified.");
                    }
                    return rootAppName;
                case UsageAppNameStyle.Executable:
                    return GetAppFileName();
                default:
                    throw new ArgumentOutOfRangeException(nameof(UsageAppNameStyle), $"unknown style: {usageAppNameStyle}");
            }
        }

        private static string GetAppNameAdaptive(ICommand app)
        {
            var rootAppName = GetRootAppName(app);
            if (rootAppName != null)
            {
                // https://github.com/bilal-fazlani/commanddotnet/issues/43
                // CommandDotNet convention:
                //   if root command has ApplicationMetadata.Name
                //   assume tool will be used as dotnet Global Tool
                //   and print usage accordingly
                return rootAppName;
            }

            string fileName = GetAppFileName();
            if (fileName != null)
            {
                // If the file is an .exe, it's more likely a .Net Framework file
                // and will not be executed via the dotnet tool.
                return fileName.EndsWith(".exe") ? fileName : $"dotnet {fileName}";
            }

            // https://github.com/bilal-fazlani/commanddotnet/issues/65
            // in some cases, entry assembly won't exist.  generally only an issue for tests.
            return "...";
        }

        private static string GetRootAppName(ICommand app)
        {
            return app.GetRootCommand().CustomAttributeProvider?.GetCustomAttribute<ApplicationMetadataAttribute>()?.Name;
        }

        private static string GetAppFileName()
        {
            var hostAssembly = Assembly.GetEntryAssembly();
            return hostAssembly == null 
                ? null 
                : Path.GetFileName(hostAssembly.Location);
        }
    }
}