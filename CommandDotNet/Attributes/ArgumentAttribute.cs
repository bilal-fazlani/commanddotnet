using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class ArgumentAttribute : ParameterBaseAttribute
    {
        public string Name { get; set; }
    }
}