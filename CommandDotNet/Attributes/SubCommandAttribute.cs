using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubCommandAttribute : Attribute
    {
        
    }
}