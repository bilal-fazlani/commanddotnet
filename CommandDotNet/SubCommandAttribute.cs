using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubCommandAttribute : Attribute
    {
        
    }
}