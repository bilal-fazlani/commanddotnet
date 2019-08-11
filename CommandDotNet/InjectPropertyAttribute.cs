using System;

namespace CommandDotNet
{
    [Obsolete("Dependencies should be injected via the ctor now and the class should be registered with your DI container")]
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectPropertyAttribute : Attribute
    {
        
    }
}