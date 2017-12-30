using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OptionAttribute : ParameterBaseAttribute
    {        
        public string ShortName { get; set; }
        
        public string LongName { get; set; }
        
        public BooleanMode BooleanMode { get; set; }
    }
}