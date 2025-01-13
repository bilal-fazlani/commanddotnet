using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>The text values provided as input to the application</summary>
[PublicAPI]
public class InputValue
{
    /// <summary>
    /// The source of these values<br/>
    /// Sources provided by this framework can be found in <see cref="Resources"/> where properties are prefixed as `Input_`
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// True if the value is from a stream and should only be enumerated in within the command.
    /// </summary>
    public bool IsStream { get; }

    private IEnumerable<string>? _values;

    /// <summary>The text values</summary>
    public IEnumerable<string>? Values
    {
        get => _values ?? ValuesFromTokens?.Select(v => v.Value);
        set => _values = value;
    }

    /// <summary>The values with tokens of origin</summary>
    public IEnumerable<ValueFromToken>? ValuesFromTokens { get; set; }

    public InputValue(string source, bool isStream, IEnumerable<string> values)
    {
        Source = source.ThrowIfNull();
        IsStream = isStream;
        Values = values.ThrowIfNull();
    }

    public InputValue(string source, bool isStream, IEnumerable<ValueFromToken> values)
    {
        Source = source.ThrowIfNull();
        IsStream = isStream;
        ValuesFromTokens = values.ThrowIfNull();
    }
}