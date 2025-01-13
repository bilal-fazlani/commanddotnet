using CommandDotNet.Extensions;
using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary>
/// A value was provided for an option that didn't expect it.<br/>
/// The option has an arity of 1 but was given multiple values<br/>
/// Or the option has an arity of 0 but was given a value.
/// </summary>
[PublicAPI]
public class UnexpectedOptionValueParseError(Command command, Option option, Token token) : IParseError
{
    public string Message { get; } = Resources.A.Parse_Unexpected_value_for_option(token.RawValue, option.Name);
    public Command Command { get; } = command.ThrowIfNull();
    public Option Option { get; } = option.ThrowIfNull();
    public Token Token { get; } = token.ThrowIfNull();
}