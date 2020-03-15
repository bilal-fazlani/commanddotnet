using System;
using System.Reflection;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// The version information for the <see cref="Assembly.GetEntryAssembly"/><br/>
    /// For unit tests, use `AppRunner.Configure(c =&gt; c.Services.Set(new VersionInfo(...)))`
    /// to set to specific version
    /// </summary>
    [Obsolete("Use AppInfo instead. It correctly detects self-contained single executable files")]
    public class VersionInfo
    {
        public string Filename { get; }
        public string Version { get; }

        public VersionInfo(string filename, string version)
        {
            Filename = filename;
            Version = version;
        }

        [Obsolete("Use AppInfo.GetAppInfo")]
        public static VersionInfo GetVersionInfo(CommandContext commandContext)
        {
            var svcs = commandContext.AppConfig.Services;
            var versionInfo = svcs.Get<VersionInfo>();
            if (versionInfo == null)
            {
                var appInfo = AppInfo.GetAppInfo(commandContext);
                svcs.AddOrUpdate(versionInfo = new VersionInfo(appInfo.FileName, appInfo.Version));
            }

            return versionInfo;
        }
    }
}