﻿using System;
using CommandDotNet.Builders;

namespace CommandDotNet.Help;

internal static class HelpBuilderExtensions
{
    internal static string GetAppName(this AppHelpSettings appHelpSettings)
    {
        if (!appHelpSettings.UsageAppName.IsNullOrEmpty())
        {
            return appHelpSettings.UsageAppName!;
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
            case UsageAppNameStyle.Executable:
                return AppInfo.Instance.FileName;
            default:
                // ReSharper disable once LocalizableElement - will be discovered in development
                throw new ArgumentOutOfRangeException(nameof(UsageAppNameStyle), $"unknown style: {appHelpSettings.UsageAppNameStyle}");
        }
    }
}