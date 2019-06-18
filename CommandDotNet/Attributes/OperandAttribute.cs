using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OperandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}