using System;
using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    public interface IParser
    {
        dynamic Parse(ArgumentInfo argumentInfo);
    }
}