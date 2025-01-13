using CommandDotNet.Extensions;
using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary>The value is not in <see cref="IArgument.AllowedValues"/></summary>
[PublicAPI]
public class NotAllowedValueParseError(Command command, IArgument argument, Token token) : IParseError
{
    public string Message { get; } = Resources.A.Parse_Unrecognized_value_for(token.RawValue,
        argument is Option ? Resources.A.Common_option_lc : Resources.A.Common_argument_lc,
        argument.Name);

    public Command Command { get; } = command.ThrowIfNull();
    public IArgument Argument { get; } = argument.ThrowIfNull();
    public Token Token { get; } = token.ThrowIfNull();
}