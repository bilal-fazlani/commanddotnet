using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// The version information for the <see cref="Assembly.GetEntryAssembly"/><br/>
    /// For unit tests, use `AppRunner.Configure(c =&gt; c.Services.Set(new VersionInfo(...)))`
    /// to set to specific version
    /// </summary>
    public class VersionInfo
    {
        public string Filename { get; }
        public string Version { get; }

        public VersionInfo(string filename, string version)
        {
            Filename = filename;
            Version = version;
        }

        public static VersionInfo GetVersionInfo(CommandContext commandContext)
        {
            var appConfigServices = commandContext.AppConfig.Services;

            var versionInfo = appConfigServices.Get<VersionInfo>();
            if (versionInfo != null)
            {
                return versionInfo;
            }

            var hostAssembly = Assembly.GetEntryAssembly();
            if (hostAssembly == null)
            {
                throw new AppRunnerException(
                    "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                    "This is a known issue when running unit tests in .net framework. https://tinyurl.com/y6rnjqsg" +
                    "Set the version info in AppRunner.Configure(c => c.Services.Set(new VersionInfo(...))) " +
                    "to create a specific version for tests");
            }

            var filename = Path.GetFileName(hostAssembly.Location);
            var fvi = FileVersionInfo.GetVersionInfo(hostAssembly.Location);
            versionInfo = new VersionInfo(filename, fvi.ProductVersion);
            appConfigServices.Set(versionInfo);
            return versionInfo;
        }
    }
}