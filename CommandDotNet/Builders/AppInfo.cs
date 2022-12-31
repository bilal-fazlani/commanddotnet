﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Extensions;
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
    public class AppInfo : ICloneable, IIndentableToString
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private static AppInfo? sAppInfo;
        private static Func<AppInfo>? sAppInfoResolver;

        /// <summary>
        /// The instance of AppInfo used by all commands.
        /// Use <see cref="SetResolver"/> to override this for consistent tests.
        /// </summary>
        public static AppInfo Instance => sAppInfoResolver?.Invoke() ?? (sAppInfo ??= BuildAppInfo());

        /// <summary>
        /// Use to override the AppInfo logic for consistent usage info in tests
        /// or override using TestConfig.AppInfoOverride
        /// https://commanddotnet.bilal-fazlani.com/testtools/harness/test-config/
        /// </summary>
        public static IDisposable SetResolver(Func<AppInfo> appInfoResolver)
        {
            sAppInfoResolver = appInfoResolver;
            return new DisposableAction(() => sAppInfoResolver = null);
        }

        private string? _version;

        /// <summary>True if the application's filename ends with .exe</summary>
        public bool IsExe { get; }

        /// <summary>True if the application is located in the global dotnet tool directory</summary>
        public bool IsGlobalTool { get; }

        /// <summary>True if published as a self-contained single executable</summary>
        public bool IsSelfContainedExe { get; }

        /// <summary>True if run using the dotnet.exe</summary>
        public bool IsRunViaDotNetExe { get; }

        /// <summary>The entry assembly. Could be an exe or dll.</summary>
        public Assembly EntryAssembly { get; }

        /// <summary>The full path to the file used to execute the app</summary>
        public string FilePath { get; }

        /// <summary>The file name used to execute the app</summary>
        public string FileName { get; }

        public string? Version => _version ??= GetVersion(Instance.EntryAssembly);

        // second ctor to avoid breaking change
        public AppInfo(
            bool isExe, bool isSelfContainedExe, bool isRunViaDotNetExe, 
            bool isGlobalTool,
            Assembly entryAssembly,
            string filePath, string fileName,
            string? version = null)
        : this(isExe, isSelfContainedExe, isRunViaDotNetExe, entryAssembly, filePath, fileName, version)
        {
            IsGlobalTool = isGlobalTool;
        }

        [Obsolete("Use ctor with isGlobalTool param")]
        public AppInfo(
            bool isExe, bool isSelfContainedExe, bool isRunViaDotNetExe,
            Assembly entryAssembly,
            string filePath, string fileName,
            string? version = null)
        {
            IsExe = isExe;
            IsSelfContainedExe = isSelfContainedExe;
            IsRunViaDotNetExe = isRunViaDotNetExe;
            EntryAssembly = entryAssembly;
            FilePath = filePath;
            FileName = fileName;
            _version = version;
        }

        private static AppInfo BuildAppInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                throw new InvalidConfigurationException(
                    "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                    "This is a known issue when running unit tests in .net framework. https://tinyurl.com/y6rnjqsg \n" +
                    "Set the version info in AppInfo.OverrideInstance(new AppInfo(...))\n" +
                    "If you are using the TestTools https://tinyurl.com/y6t4oxra, you can override using TestConfig.AppInfoOverride.");
            }

            // this could be dotnet.exe or {app_name}.exe if published as single executable
            var mainModule = Process.GetCurrentProcess().MainModule;
            var mainModuleFilePath = mainModule?.FileName;
            var entryAssemblyFilePath = entryAssembly.Location;
            
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

            var isRunViaDotNetExe = false;
            var isSelfContainedExe = false;
            var isExe = false;
            var isGlobalTool = false;
            if (mainModuleFileName is not null)
            {
                // osx uses 'dotnet' instead of 'dotnet.exe'
                isRunViaDotNetExe = mainModuleFileName.Equals("dotnet.exe") || mainModuleFileName.Equals("dotnet");
                if (!isRunViaDotNetExe)
                {
                    var entryAssemblyFileNameWithoutExt = Path.GetFileNameWithoutExtension(entryAssemblyFileName);
                    isSelfContainedExe = isExe = mainModuleFileName.EndsWith($"{entryAssemblyFileNameWithoutExt}.exe");
                }

                var globalToolsDirectory = DotNetSuggest.DotNetTools.GlobalToolDirectory;
                isGlobalTool = globalToolsDirectory is not null 
                               && mainModuleFilePath!.StartsWith(globalToolsDirectory);
            }

            isExe = isExe || entryAssemblyFileName.EndsWith("exe");

            var filePath = isSelfContainedExe
                ? mainModuleFilePath
                : entryAssemblyFilePath;

            var fileName = Path.GetFileName(filePath)!;

            Log.Debug($"  {nameof(FileName)}={fileName} " +
                      $"{nameof(IsRunViaDotNetExe)}={isRunViaDotNetExe} " +
                      $"{nameof(IsSelfContainedExe)}={isSelfContainedExe} " +
                      $"{nameof(IsGlobalTool)}={isGlobalTool} " +
                      $"{nameof(FilePath)}={filePath}");

            return new AppInfo(isExe, isSelfContainedExe, isRunViaDotNetExe, isGlobalTool, entryAssembly, filePath!, fileName);
        }

        private static string? GetVersion(Assembly hostAssembly) =>
            // thanks Spectre console for figuring this out https://github.com/spectreconsole/spectre.console/issues/242
            hostAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "?";

        public object Clone() => new AppInfo(IsExe, IsSelfContainedExe, IsRunViaDotNetExe, IsGlobalTool, EntryAssembly, FilePath, FileName, _version);

        public override string ToString() => ToString(new Indent());

        public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
    }
}