using System.Collections.Generic;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing;

internal class SingleValueParser(IArgumentTypeDescriptor argumentTypeDescriptor) : IParser
{
    public object? Parse(IArgument argument, IEnumerable<string> values)
    {
        var value = values.SingleOrDefaultOrThrow(() => ThrowMultiForSingleEx(argument));
        return value is null
            ? null
            : argumentTypeDescriptor.ParseString(argument, value);
    }

    private static void ThrowMultiForSingleEx(IArgument argument) =>
        throw new ValueParsingException(
            Resources.A.Parse_ArgumentArity_Expected_single_value(argument.Name));
}