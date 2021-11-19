using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubcommandAttribute : Attribute
    {

    }

    [Obsolete("use SubcommandAttribute instead. Lowercase C")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubCommandAttribute : SubcommandAttribute
    {

    }
}