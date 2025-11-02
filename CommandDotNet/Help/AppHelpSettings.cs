using System;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.Help;

[PublicAPI]
public class AppHelpSettings : IIndentableToString
{
    private ExecutionAppSettings? _executionSettings;

    /// <summary>When true, the help option will be included in the help for every command</summary>
    public bool PrintHelpOption { get; set; }

    /// <summary>Specify whether to use Basic or Detailed help mode. Default is Detailed.</summary>
    public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;
    
    internal void SetExecutionSettings(ExecutionAppSettings executionSettings) => _executionSettings = executionSettings;

    /// <summary>Specify what AppName to use in the 'Usage:' example</summary>
    [Obsolete("Use AppSettings.Execution.UsageAppNameStyle")]
    public UsageAppNameStyle UsageAppNameStyle
    {
        get => _executionSettings?.UsageAppNameStyle ?? UsageAppNameStyle.Adaptive;
        set { if (_executionSettings != null) _executionSettings.UsageAppNameStyle = value; }
    }

    /// <summary>
    /// The application name used in the Usage section of help documentation.<br/>
    /// When specified, <see cref="UsageAppNameStyle"/> is ignored.
    /// </summary>
    [Obsolete("Use AppSettings.Execution.UsageAppName")]
    public string? UsageAppName
    {
        get => _executionSettings?.UsageAppName;
        set { if (_executionSettings != null) _executionSettings.UsageAppName = value; }
    }

    /// <summary>
    /// When true, the usage section will expand arguments so the names of all arguments are shown.
    /// </summary>
    public bool ExpandArgumentsInUsage { get; set; } = true;

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
}