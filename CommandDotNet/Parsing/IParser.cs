using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    internal interface IParser
    {
        dynamic Parse(ArgumentInfo argumentInfo);
    }
}