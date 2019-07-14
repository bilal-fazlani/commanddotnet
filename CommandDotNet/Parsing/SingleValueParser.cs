using System;
using System.Collections.Generic;
using System.Linq;
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

        public object Parse(IArgument argument, List<string> values)
        {
            return ParseString(argument, values.SingleOrDefault());
        }

        public object ParseString(IArgument argument, string value)
        {
            try
            {
                return _argumentTypeDescriptor.ParseString(argument, value);
            }
            catch (FormatException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_argumentTypeDescriptor.GetDisplayName(argument)}");
            }
            catch (ArgumentException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_argumentTypeDescriptor.GetDisplayName(argument)}");
            }
        }
    }
}