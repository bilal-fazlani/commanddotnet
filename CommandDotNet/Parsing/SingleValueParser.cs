using System.Collections.Generic;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing
{
    internal class SingleValueParser : IParser
    {
        private readonly IArgumentTypeDescriptor _argumentTypeDescriptor;

        public SingleValueParser(IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _argumentTypeDescriptor = argumentTypeDescriptor;
        }

        public object? Parse(IArgument argument, IEnumerable<string> values)
        {
            var value = values.SingleOrDefaultOrThrow(() => ThrowMultiForSingleEx(argument));
            return value is null
                ? null
                : _argumentTypeDescriptor.ParseString(argument, value);
        }

        private static void ThrowMultiForSingleEx(IArgument argument)
        {
            throw new ValueParsingException(
                Resources.A.Parse_ArgumentArity_Expected_single_value(argument.Name));
        }
    }
}