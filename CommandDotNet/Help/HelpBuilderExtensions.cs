using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Help
{
    internal static class HelpBuilderExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static string GetAppName(this Command command, AppHelpSettings appHelpSettings)
        {
            if (!appHelpSettings.UsageAppName.IsNullOrEmpty())
            {
                return appHelpSettings.UsageAppName;
            }

            switch (appHelpSettings.UsageAppNameStyle)
            {
                case UsageAppNameStyle.Adaptive:
                    return GetAppNameAdaptive();
                case UsageAppNameStyle.DotNet:
                    return $"dotnet {GetAppFileName()}";
                case UsageAppNameStyle.GlobalTool:
                    var rootAppName = GetRootAppName(command);
                    if (rootAppName == null)
                    {
                        throw new AppRunnerException(
                            $"Invalid configuration: {nameof(CommandAttribute)}.{nameof(CommandAttribute.Name)} " +
                            $"is required for the root command when {nameof(UsageAppNameStyle)}.{nameof(UsageAppNameStyle.GlobalTool)} " +
                            "is specified.");
                    }
                    return rootAppName;
                case UsageAppNameStyle.Executable:
                    return GetAppFileName();
                default:
                    throw new ArgumentOutOfRangeException(nameof(UsageAppNameStyle), $"unknown style: {appHelpSettings.UsageAppNameStyle}");
            }
        }

        private static string GetAppNameAdaptive()
        {
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

        private static string GetRootAppName(Command command)
        {
            return command.GetRootCommand().CustomAttributes?.GetCustomAttribute<CommandAttribute>()?.Name;
        }

        private static string GetAppFileName()
        {
            var mainModuleFilePath = Process.GetCurrentProcess().MainModule?.FileName;
            var hostAssemblyFilePath = Assembly.GetEntryAssembly()?.Location;

            Log.Debug($"{nameof(mainModuleFilePath)}: {mainModuleFilePath}");
            Log.Debug($"{nameof(hostAssemblyFilePath)}: {hostAssemblyFilePath}");
            
            var mainModuleFileName = Path.GetFileName(mainModuleFilePath);
            var assemblyFileName = Path.GetFileName(hostAssemblyFilePath);

            if (mainModuleFileName == null || mainModuleFileName.Equals("dotnet.exe") || assemblyFileName.EndsWith("exe"))
            {
                return assemblyFileName;
            }

            if (hostAssemblyFilePath.EndsWith(".dll") &&
                mainModuleFilePath.EndsWith($"{Path.GetFileNameWithoutExtension(hostAssemblyFilePath)}.exe"))
            {
                // app is published as a single file
                return mainModuleFileName;
            }

            return assemblyFileName;
        }
    }
}