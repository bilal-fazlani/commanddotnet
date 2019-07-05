using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {        
        public string ShortName { get; set; }
        
        [Obsolete("Use Name instead")]
        public string LongName
        {
            get => Name; 
            set => Name = value;
        }

        public string Name { get; set; }

        public BooleanMode BooleanMode { get; set; }
        
        public bool Inherited { get; set; }

        public string Description { get; set; }
    }
}