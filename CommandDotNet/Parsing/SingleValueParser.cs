using System;
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

        public object Parse(IArgument argument, IEnumerable<string> values)
        {
            return _argumentTypeDescriptor.ParseString(argument, 
                values.SingleOrDefaultOrThrow(() => ThrowMultiForSingleEx(argument)));
        }

        private static void ThrowMultiForSingleEx(IArgument argument)
        {
            var message = $"{argument.GetType().Name}:{argument.Name} accepts only a single value but multiple values were provided";
            throw new CommandParsingException(argument.Parent, message);
        }
    }
}