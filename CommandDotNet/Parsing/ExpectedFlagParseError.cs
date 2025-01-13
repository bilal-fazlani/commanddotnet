using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

[PublicAPI]
public record ExpectedFlagParseError(
    Command Command,
    Token ClubbedToken,
    string ShortName,
    Option? Option,
    string Message)
    : IParseError;