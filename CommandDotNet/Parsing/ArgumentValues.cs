using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    public class ArgumentValues
    {
        public IArgument Argument { get; }
        public List<string> Values { get; }

        public ArgumentValues(IArgument argument, List<string> values)
        {
            Argument = argument;
            Values = values;
        }

        public override string ToString()
        {
            return $"{nameof(ArgumentValues)}:{Argument.Name}={Values.ToCsv(", ")}";
        }
    }
}