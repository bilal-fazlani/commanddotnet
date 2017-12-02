using System;

namespace CommandDotNet.Attributes
{
    public class ArguementAttribute : Attribute
    {
        public string Description { get; set; }

        public bool RequiredString { get; set; }
        
        public string Template { get; set; }
    }
}