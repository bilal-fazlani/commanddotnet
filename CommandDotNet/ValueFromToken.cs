using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public class ValueFromToken(string value, Token? valueToken, Token? optionToken)
{
    public string Value { get; } = value;
    public Token? ValueToken { get; } = valueToken;
    public Token? OptionToken { get; } = optionToken;

    public string? TokenSourceName => ValueToken?.SourceName ?? OptionToken?.SourceName;
    public Token? TokensSourceToken => ValueToken?.SourceToken ?? OptionToken?.SourceToken;

    public override string ToString() => 
        $"{nameof(ValueFromToken)}={Value} {nameof(ValueToken)}:{ValueToken} {nameof(OptionToken)}:{OptionToken}";
}