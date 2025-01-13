using CommandDotNet.Extensions;
using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary>
/// <see cref="Token"/> is not a valid command
/// and there is no available operand to assign the value to.
/// </summary>
[PublicAPI]
public class UnrecognizedArgumentParseError(Command command, Token token, string? optionPrefix, string message)
    : IParseError
{
    public string Message { get; } = message.ThrowIfNull();
    public Command Command { get; } = command.ThrowIfNull();
    public Token Token { get; } = token.ThrowIfNull();
    public string? OptionPrefix { get; } = optionPrefix;
}