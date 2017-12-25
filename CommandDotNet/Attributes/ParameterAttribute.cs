using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : ArgumentBaseAttribute
    {
        public bool RequiredString { get; set; }
    }
}