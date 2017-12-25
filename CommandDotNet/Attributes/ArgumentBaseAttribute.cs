using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentBaseAttribute : Attribute
    {
        public BooleanMode BooleanMode { get; set; }
        
        public string Description { get; set; }
    }
}