using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Logging;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// The version and file information for the application.<br/>
    /// Uses <see cref="Assembly.GetEntryAssembly"/> and determines if app is run via
    /// dotnet console app or as an exe or as a self-contained single executable<br/>
    /// For unit tests, use `AppRunner.Configure(c =&gt; c.Services.Set(new AppInfo(...)))`
    /// to set to specific version
    /// </summary>
    public class AppInfo
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private static AppInfo s_appInfo;

        internal static AppInfo Instance => s_appInfo ?? (s_appInfo = BuildAppInfo());

        private string _version;

        /// <summary>The entry assembly. Could be an exe or dll.</summary>
        private Assembly _entryAssembly;

        /// <summary>
        /// The exe hosting the assembly.
        /// Could be dotnet.exe, single executable containing zipped dll or the entry assembly.
        /// </summary>
        private ProcessModule _mainModule;

        /// <summary>True the app is an executable, whether as a self-contained single executable or otherwise</summary>
        private bool _isExe;

        /// <summary>True if published as a self-contained single executable</summary>
        public bool IsSelfContainedExe { get; set; }

        /// <summary>True if run using the dotnet.exe</summary>
        public bool IsRunViaDotNetExe { get; set; }

        /// <summary>The file name used to execute the app</summary>
        public string FilePath { get; private set; }

        /// <summary>The file name used to execute the app</summary>
        public string FileName { get; private set; }

        public string Version => _version ?? (_version = GetVersion(_entryAssembly));

        private AppInfo()
        {
        }

        public AppInfo(string fileName, string version)
        {
            FileName = fileName;
            _version = version;
        }
        public static AppInfo GetAppInfo(CommandContext commandContext)
        {
            var svcs = commandContext.AppConfig.Services;
            var appInfo = svcs.Get<AppInfo>();
            if (appInfo == null)
            {
                svcs.AddOrUpdate(appInfo = AppInfo.Instance);
            }
            return appInfo;
        }

        private static AppInfo BuildAppInfo()
        {
            var appInfo = new AppInfo
            {
                _entryAssembly = Assembly.GetEntryAssembly()
            };
            if (appInfo._entryAssembly == null)
            {
                throw new AppRunnerException(
                    "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                    "This is a known issue when running unit tests in .net framework. https://tinyurl.com/y6rnjqsg" +
                    "Set the version info in AppRunner.Configure(c => c.Services.Set(new AppInfo(...))) " +
                    "to create a specific info for tests");
            }

            // this could be dotnet.exe or {app_name}.exe if published as single executable
            appInfo._mainModule = Process.GetCurrentProcess().MainModule;
            var mainModuleFilePath = appInfo._mainModule?.FileName;
            var entryAssemblyFilePath = appInfo._entryAssembly?.Location;
            
            Log.Debug($"{nameof(mainModuleFilePath)}: {mainModuleFilePath}");
            Log.Debug($"{nameof(entryAssemblyFilePath)}: {entryAssemblyFilePath}");

            var mainModuleFileName = Path.GetFileName(mainModuleFilePath);
            var entryAssemblyFileName = Path.GetFileName(entryAssemblyFilePath);

            // this logic isn't not covered by unit tests.
            // changing this logic requires manual testing of
            // - .dll files run in dotnet
            // - .exe console apps
            // - .dll published as self-contained .exe files.
            // - windows, linux & mac

            if (mainModuleFileName != null)
            {
                // osx uses 'dotnet' instead of 'dotnet.exe'
                if (!(appInfo.IsRunViaDotNetExe = mainModuleFileName.Equals("dotnet.exe") || mainModuleFileName.Equals("dotnet")))
                {
                    var entryAssemblyFileNameWithoutExt = Path.GetFileNameWithoutExtension(entryAssemblyFileName);
                    appInfo.IsSelfContainedExe = appInfo._isExe = mainModuleFileName.EndsWith($"{entryAssemblyFileNameWithoutExt}.exe");
                }
            }

            appInfo._isExe = appInfo._isExe || entryAssemblyFileName.EndsWith("exe");

            appInfo.FilePath = appInfo.IsSelfContainedExe
                ? mainModuleFilePath
                : entryAssemblyFilePath;

            appInfo.FileName = Path.GetFileName(appInfo.FilePath);

            Log.Debug($"  {nameof(FileName)}={appInfo.FileName} " +
                      $"{nameof(IsRunViaDotNetExe)}={appInfo.IsRunViaDotNetExe} " +
                      $"{nameof(IsSelfContainedExe)}={appInfo.IsSelfContainedExe} " +
                      $"{nameof(FilePath)}={appInfo.FilePath}");

            return appInfo;
        }

        internal static string GetVersion(Assembly hostAssembly)
        {
            var filename = Path.GetFileName(hostAssembly.Location);
            var fvi = FileVersionInfo.GetVersionInfo(hostAssembly.Location);
            return fvi.ProductVersion;
        }
    }
}