using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Logging;
using JetBrains.Annotations;

namespace CommandDotNet.Builders;

/// <summary>
/// The version and file information for the application.<br/>
/// Uses <see cref="Assembly.GetEntryAssembly"/> and determines if app is run via
/// dotnet console app or as an exe or as a self-contained single executable<br/>
/// For unit tests, use `AppRunner.Configure(c =&gt; c.Services.Set(new AppInfo(...)))`
/// to set to specific version
/// </summary>
[PublicAPI]
public class AppInfo(
    bool isExe,
    bool isSelfContainedExe,
    bool isRunViaDotNetExe,
    Assembly entryAssembly,
    string filePath,
    string fileName,
    string? version = null)
    : ICloneable, IIndentableToString
{
    private static readonly ILog Log = LogProvider.GetLogger(typeof(AppInfo));

    private static AppInfo? _appInfo;
    private static Func<AppInfo>? _appInfoResolver;

    /// <summary>
    /// The instance of AppInfo used by all commands.
    /// Use <see cref="SetResolver"/> to override this for consistent tests.
    /// </summary>
    public static AppInfo Instance => _appInfoResolver?.Invoke() ?? (_appInfo ??= BuildAppInfo());

    /// <summary>
    /// Use to override the AppInfo logic for consistent usage info in tests
    /// or override using TestConfig.AppInfoOverride
    /// https://commanddotnet.bilal-fazlani.com/testtools/harness/test-config/
    /// </summary>
    public static IDisposable SetResolver(Func<AppInfo> appInfoResolver)
    {
        _appInfoResolver = appInfoResolver;
        return new DisposableAction(() => _appInfoResolver = null);
    }

    private string? _version = version;

    /// <summary>True if the application's filename ends with .exe</summary>
    public bool IsExe { get; } = isExe;

    /// <summary>True if published as a self-contained single executable</summary>
    public bool IsSelfContainedExe { get; } = isSelfContainedExe;

    /// <summary>True if run using the dotnet.exe</summary>
    public bool IsRunViaDotNetExe { get; } = isRunViaDotNetExe;

    /// <summary>The entry assembly. Could be an exe or dll.</summary>
    public Assembly EntryAssembly { get; } = entryAssembly;

    /// <summary>The full path to the file used to execute the app</summary>
    public string FilePath { get; } = filePath;

    /// <summary>The file name used to execute the app</summary>
    public string FileName { get; } = fileName;

    public string? Version => _version ??= GetVersion(Instance.EntryAssembly);

    /// <summary>
    /// Gets the application executable name to use in usage examples and generated scripts.
    /// Uses <see cref="ExecutionAppSettings.UsageAppName"/> if explicitly set,
    /// otherwise determines the name based on <see cref="ExecutionAppSettings.UsageAppNameStyle"/>.
    /// The result may include "dotnet" prefix depending on the style (e.g., "dotnet myapp.dll" or "myapp").
    /// </summary>
    public static string GetExecutableAppName(ExecutionAppSettings executionSettings)
    {
        if (!executionSettings.UsageAppName.IsNullOrEmpty())
        {
            return executionSettings.UsageAppName!;
        }

        switch (executionSettings.UsageAppNameStyle)
        {
            case UsageAppNameStyle.Adaptive:
                var appInfo = Instance;
                return appInfo.IsRunViaDotNetExe
                    ? $"dotnet {appInfo.FileName}"
                    : appInfo.FileName;
            case UsageAppNameStyle.DotNet:
                return $"dotnet {Instance.FileName}";
            case UsageAppNameStyle.Executable:
                return Instance.FileName;
            default:
                // ReSharper disable once LocalizableElement - will be discovered in development
                throw new ArgumentOutOfRangeException(nameof(UsageAppNameStyle), $"unknown style: {executionSettings.UsageAppNameStyle}");
        }
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
            
        // warning IL3000: 'System.Reflection.Assembly.Location' always returns
        // an empty string for assemblies embedded in a single-file app.
        // If the path to the app directory is needed, consider calling 'System.AppContext.BaseDirectory'
        var entryAssemblyFilePath = entryAssembly.Location;
        if (entryAssemblyFilePath.IsNullOrEmpty())
        {
            // https://github.com/dotnet/corert/issues/5467#issuecomment-369202524
            entryAssemblyFilePath = Path.Combine(AppContext.BaseDirectory, entryAssembly.ManifestModule.Name);
                
            // if this still fails for an app, specify for Assembly.Location to returns paths to (non-existent)
            // files in AppContext.BaseDirectory using...
            // AppContext.SetSwitch("Switch.System.Reflection.Assembly.SimulatedLocationInBaseDirectory", true);
            // https://github.com/dotnet/corert/issues/5467#issuecomment-464611875
        }
            
        Log.Debug($"{nameof(mainModuleFilePath)}: {mainModuleFilePath}");
        Log.Debug($"{nameof(entryAssemblyFilePath)}: {entryAssemblyFilePath}");

        var mainModuleFileName = Path.GetFileName(mainModuleFilePath);
        var entryAssemblyFileName = Path.GetFileName(entryAssemblyFilePath);

        // this logic is not covered by unit tests.
        // changing this logic requires manual testing of
        // - .dll files run in dotnet
        // - .exe console apps
        // - .dll published as self-contained .exe files.
        // - windows, linux & mac

        var isRunViaDotNetExe = false;
        var isSelfContainedExe = false;
        var isExe = false;
        if (mainModuleFileName != null)
        {
            // osx uses 'dotnet' instead of 'dotnet.exe'
            if (!(isRunViaDotNetExe = mainModuleFileName.Equals("dotnet.exe") || mainModuleFileName.Equals("dotnet")))
            {
                var entryAssemblyFileNameWithoutExt = Path.GetFileNameWithoutExtension(entryAssemblyFileName);
                isSelfContainedExe = isExe = mainModuleFileName.EndsWith($"{entryAssemblyFileNameWithoutExt}.exe");
            }
        }

        isExe = isExe || entryAssemblyFileName.EndsWith("exe");

        var filePath = isSelfContainedExe
            ? mainModuleFilePath
            : entryAssemblyFilePath;

        var fileName = Path.GetFileName(filePath)!;

        Log.Debug($"  {nameof(FileName)}={fileName} " +
                  $"{nameof(IsRunViaDotNetExe)}={isRunViaDotNetExe} " +
                  $"{nameof(IsSelfContainedExe)}={isSelfContainedExe} " +
                  $"{nameof(FilePath)}={filePath}");

        return new AppInfo(isExe, isSelfContainedExe, isRunViaDotNetExe, entryAssembly, filePath!, fileName);
    }

    private static string? GetVersion(Assembly hostAssembly)
    {
        // thanks Spectre console for figuring this out https://github.com/spectreconsole/spectre.console/issues/242
        var version = hostAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (version is null)
        {
            return null;
        }
        var indexOfBuildInfo = version.IndexOf('+');
        return indexOfBuildInfo > 0 ? version[..indexOfBuildInfo] : version;
    }

    public object Clone() => new AppInfo(IsExe, IsSelfContainedExe, IsRunViaDotNetExe, EntryAssembly, FilePath, FileName, _version);

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
}