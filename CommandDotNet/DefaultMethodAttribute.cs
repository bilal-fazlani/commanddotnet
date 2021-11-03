using System;

namespace CommandDotNet
{
    [Obsolete("Use DefaultCommandAttribute instead")]
    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultMethodAttribute : DefaultCommandAttribute
    {

    }
}