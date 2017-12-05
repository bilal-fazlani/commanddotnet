using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentAttribute : Attribute
    {
        public string ShortName { get; set; }
        
        public string LongName { get; set; }
        
        public string Description { get; set; }

        public bool RequiredString { get; set; }
    }
}