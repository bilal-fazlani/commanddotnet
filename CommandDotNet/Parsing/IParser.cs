using System.Collections.Generic;
using CommandDotNet.ClassModeling;

namespace CommandDotNet.Parsing
{
    internal interface IParser
    {
        object Parse(IArgument argument, List<string> values);
        dynamic Parse(ArgumentInfo argumentInfo);
    }
}