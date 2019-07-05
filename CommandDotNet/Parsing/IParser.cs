using CommandDotNet.ClassModeling;

namespace CommandDotNet.Parsing
{
    internal interface IParser
    {
        dynamic Parse(ArgumentInfo argumentInfo);
    }
}