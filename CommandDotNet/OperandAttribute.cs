using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OperandAttribute : Attribute, INameAndDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}