using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class VersionService
    {
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