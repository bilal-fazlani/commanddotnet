using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {        
        public string ShortName { get; set; }
        
        public string LongName { get; set; }
        
        public BooleanMode BooleanMode { get; set; }
        
        public bool Inherited { get; set; }

        public string Description { get; set; }
    }
}