using System;
using CommandDotNet.ClassModeling;
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
        
        public dynamic Parse(ArgumentInfo argumentInfo)
        {
            var value = argumentInfo.ValueInfo.Value;
            return ParseString(argumentInfo, value);
        }

        public dynamic ParseString(ArgumentInfo argumentInfo, string value)
        {
            try
            {
                return _argumentTypeDescriptor.ParseString(argumentInfo, value);
            }
            catch (FormatException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_argumentTypeDescriptor.GetDisplayName(argumentInfo)}");
            }
            catch (ArgumentException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_argumentTypeDescriptor.GetDisplayName(argumentInfo)}");
            }
        }
    }
}