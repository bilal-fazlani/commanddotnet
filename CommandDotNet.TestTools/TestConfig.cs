using System;
using CommandDotNet.Builders;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.TestTools;

[PublicAPI]
public class TestConfig
{
    // TODO: look for CommandDotNet.TestTools.json file

    private static TestConfig? _defaultTestConfig;

    /// <summary>
    /// Default scans loaded assemblies for <see cref="IDefaultTestConfig"/>
    /// and stores the config with the lowest <see cref="Priority"/>
    /// </summary>
    public static TestConfig Default
    {
        get => _defaultTestConfig ??= TestConfigFactory.GetDefaultFromSubClass() ?? new TestConfig{Source = "TestConfig.Default"};
        set => _defaultTestConfig = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Nothing will be printed and errors will be captured</summary>
    public static TestConfig Silent { get; set; } = new()
    {
        OnSuccess = new OnSuccessConfig(), 
        OnError = new OnErrorConfig(),
        Source = "TestConfig.Silent"
    };

    /// <summary>
    /// Configuration to be used when no exception has escaped <see cref="AppRunner.Run"/><br/>
    /// Default: prints nothing
    /// </summary>
    public OnSuccessConfig OnSuccess { get; set; } = new();

    /// <summary>
    /// Configuration to be used when no exception has escaped <see cref="AppRunner.Run"/><br/>
    /// Default: prints <see cref="PrintConfig.ConsoleOutput"/>
    /// </summary>
    public OnErrorConfig OnError { get; set; } = new()
    {
        Print = { ConsoleOutput = true }
    };

    /// <summary>When true, CommandDotNet logs will output to logLine</summary>
    public bool PrintCommandDotNetLogs { get; set; }

    /// <summary>When true, CommandDotNet will not trim the end of the outputs from TestConsole</summary>
    public bool SkipTrimEndOfConsoleOutputs { get; set; }

    /// <summary>
    /// To identify the <see cref="TestConfig"/> in case the expected config was not used.<br/>
    /// Will be auto-populated when created from <seealso cref="IDefaultTestConfig"/>
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// When multiple <see cref="IDefaultTestConfig"/>s are found,
    /// the <see cref="TestConfig"/> with the lowest priority will be used.<br/>
    /// This property is only needed when providing the default via <see cref="IDefaultTestConfig"/><br/>
    /// Set <see cref="Default"/> directly to avoid use of this property.<br/>
    /// Create and .gitignore a <see cref="IDefaultTestConfig"/> with short.MinValue
    /// for verbose local logging and quite CI logging.<br/>
    /// </summary>
    public short? Priority { get; set; }

    /// <summary>
    /// Used to override the <see cref="AppInfo"/> used by tests.
    /// This will ensure consistent results when verifying the Usage
    /// section of the output.
    /// </summary>
    public AppInfo? AppInfoOverride { get; set; }

    [PublicAPI]
    public class OnSuccessConfig
    {
        public PrintConfig Print { get; set; } = new();
    }

    [PublicAPI]
    public class OnErrorConfig
    {
        /// <summary>
        /// When true, errors escaping <see cref="AppRunner.Run"/> will be
        /// captured in <see cref="AppRunnerResult"/> and <see cref="AppRunnerResult.ExitCode"/>
        /// will be set to 1. This mimics how the shell will process it.
        /// </summary>
        public bool CaptureAndReturnResult { get; set; }
        public PrintConfig Print { get; set; } = new();
    }

    [PublicAPI]
    public class PrintConfig
    {
        /// <summary>When true, all options will be printed</summary>
        public bool All
        {
            get => AppConfig && CommandContext && ConsoleOutput && ParseReport;
            set => AppConfig = CommandContext = ConsoleOutput = ParseReport = value;
        }

        /// <summary>Print the <see cref="AppConfig"/></summary>
        public bool AppConfig { get; set; }

        /// <summary>Print the <see cref="CommandContext"/></summary>
        public bool CommandContext { get; set; }

        /// <summary>Print the <see cref="ITestConsole.AllText"/></summary>
        public bool ConsoleOutput { get; set; }

        /// <summary>
        /// Print the output of <see cref="ParseReporter.Report"/> to see how values are assigned to arguments
        /// </summary>
        public bool ParseReport { get; set; }
    }

    /// <summary>Clones the TestConfig and applies the <see cref="alter"/> action</summary>
    public TestConfig Where(Action<TestConfig> alter)
    {
        ArgumentNullException.ThrowIfNull(alter);

        var config = (TestConfig) this.CloneWithPublicProperties();
        alter(config);
        return config;
    }

    public override string ToString() => $"{nameof(TestConfig)}:{Source}";
}