using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PipedInputAttribute : Attribute
    {
        public bool KeepEmptyLines { get; }

        public PipedInputAttribute(bool keepEmptyLines = false)
        {
            KeepEmptyLines = keepEmptyLines;
        }
    }
}