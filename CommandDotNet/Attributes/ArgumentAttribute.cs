using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentAttribute : ParameterBaseAttribute
    {
        public string Name { get; set; }
    }
}