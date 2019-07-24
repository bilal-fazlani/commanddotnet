using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {        
        public string ShortName { get; set; }
        
        public string LongName { get; set; }

        string INameAndDescription.Name => LongName ?? ShortName;

        public BooleanMode BooleanMode { get; set; }
        
        public bool Inherited { get; set; }

        public string Description { get; set; }
    }
}