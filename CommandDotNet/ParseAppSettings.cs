using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public class ParseAppSettings : IIndentableToString
{
    // begin-snippet: ParseAppSettings
    /// <summary>
    /// The default <see cref="ArgumentSeparatorStrategy"/>.
    /// This can be overridden for a <see cref="Command"/> using the <see cref="CommandAttribute"/>
    /// </summary>
    public ArgumentSeparatorStrategy DefaultArgumentSeparatorStrategy { get; set; } = ArgumentSeparatorStrategy.EndOfOptions;

    /// <summary>
    /// When false, unexpected operands will generate a parse failure.<br/>
    /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
    /// </summary>
    public bool IgnoreUnexpectedOperands { get; set; }
        
    public bool AllowBackslashOptionPrefix { get; set; }

    public bool AllowSingleHyphenForLongNames { get; set; }
    // end-snippet

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
}