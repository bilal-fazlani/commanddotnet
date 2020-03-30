using System;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;

namespace CommandDotNet.Help
{
    internal static class HelpBuilderExtensions
    {
        internal static string GetAppName(this Command command, AppHelpSettings appHelpSettings)
        {
            if (!appHelpSettings.UsageAppName.IsNullOrEmpty())
            {
                return appHelpSettings.UsageAppName;
            }

            switch (appHelpSettings.UsageAppNameStyle)
            {
                case UsageAppNameStyle.Adaptive:
                    var appInfo = AppInfo.Instance;
                    return appInfo.IsRunViaDotNetExe
                        ? $"dotnet {appInfo.FileName}"
                        : appInfo.FileName;
                case UsageAppNameStyle.DotNet:
                    return $"dotnet {AppInfo.Instance.FileName}";
                case UsageAppNameStyle.GlobalTool:
                    var rootAppName = command.GetRootCommand()
                        .GetCustomAttribute<CommandAttribute>()?
                        .Name;
                    if (rootAppName == null)
                    {
                        throw new AppRunnerException(
                            $"Invalid configuration: {nameof(CommandAttribute)}.{nameof(CommandAttribute.Name)} " +
                            $"is required for the root command when {nameof(UsageAppNameStyle)}.{nameof(UsageAppNameStyle.GlobalTool)} " +
                            $"is specified.{Environment.NewLine}" +
                            $"Use {nameof(AppSettings)}.{nameof(AppSettings.Help)}.{nameof(AppHelpSettings.UsageAppName)} " +
                            $"instead of {nameof(UsageAppNameStyle)}.{nameof(UsageAppNameStyle.GlobalTool)}.");
                    }
                    return rootAppName;
                case UsageAppNameStyle.Executable:
                    return AppInfo.Instance.FileName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(UsageAppNameStyle), $"unknown style: {appHelpSettings.UsageAppNameStyle}");
            }
        }
    }
}