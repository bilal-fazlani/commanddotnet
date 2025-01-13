using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary>An option was specified without a value</summary>
[PublicAPI]
public class MissingOptionValueParseError(Command command, Option option) : IParseError
{
    public string Message { get; } = Resources.A.Parse_Missing_value_for_option(option.Name);
    public Command Command { get; } = command.ThrowIfNull();
    public Option Option { get; } = option.ThrowIfNull();
}