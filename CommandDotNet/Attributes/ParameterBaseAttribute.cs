using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterBaseAttribute : Attribute
    {
        public string Description { get; set; }
    }
}