using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>The values provided as the default for an argument</summary>
[PublicAPI]
public class ArgumentDefault(string source, string key, object value)
{
    /// <summary>The source of the default value</summary>
    public string Source { get; } = source.ThrowIfNull();

    /// <summary>The key of the default value</summary>
    public string Key { get; } = key.ThrowIfNull();

    /// <summary>The text values</summary>
    public object Value { get; } = value.ThrowIfNull();

    public override string ToString() =>
        // do not include value in case it's a password
        $"{nameof(ArgumentDefault)}: {Source}.{Key}";
}