using JetBrains.Annotations;

namespace CommandDotNet.Tokens;

[PublicAPI]
public class Token(
    string rawValue,
    string value,
    TokenType tokenType)
{
    /// <summary>
    /// The raw value from the user input.
    /// This will contain the punctuation used to denote option and argument names.
    /// </summary>
    public string RawValue { get; } = rawValue;

    /// <summary>Can be an Option name or an argument value</summary>
    public string Value { get; } = value;

    /// <summary>The <see cref="Tokens.TokenType"/></summary>
    public TokenType TokenType { get; } = tokenType;

    public string? SourceName { get; internal set; }

    public Token? SourceToken { get; internal set; }

    public override string ToString() => $"{nameof(Token)}:{TokenType}>{RawValue}";
}