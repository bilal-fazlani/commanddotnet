using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary>
/// <see cref="Token"/> is not a valid command
/// and there is no available operand to assign the value to.
/// </summary>
[PublicAPI]
public class UnrecognizedOptionParseError(Command command, Token token, string optionPrefix, string? message = null)
    : UnrecognizedArgumentParseError(command, token, optionPrefix,
        message ?? Resources.A.Parse_Unrecognized_option(token.RawValue));