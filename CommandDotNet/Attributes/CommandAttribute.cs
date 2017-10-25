using System;

namespace CommandDotNet.Attributes
{
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
//        public string Name { get; set; }
    }
}