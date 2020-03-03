using System;

namespace CommandDotNet
{
    [Obsolete("Use OperandAttribute instead")]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute, INameAndDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}