using System;

namespace CommandDotNet.Attributes
{
    public class ConsoleApplicationAttribute : Attribute
    {
        public string Description { get; set; }
    }
}