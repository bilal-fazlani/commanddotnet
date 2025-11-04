using CommandDotNet.Extensions;
using CommandDotNet.Help;
using JetBrains.Annotations;

namespace CommandDotNet.Execution;

/// <summary>Settings specific to application execution and invocation</summary>
[PublicAPI]
public class ExecutionAppSettings : IIndentableToString
{
    // begin-snippet: ExecutionAppSettings
    /// <summary>Specify what AppName to use in usage examples, help text, and generated scripts</summary>
    public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

    /// <summary>
    /// The application name used in usage examples, help text, and generated scripts.<br/>
    /// When specified, <see cref="UsageAppNameStyle"/> is ignored.
    /// </summary>
    public string? UsageAppName { get; set; }
    // end-snippet

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
}
