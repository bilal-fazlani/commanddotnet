using System;

namespace CommandDotNet.TypeDescriptors
{
    public interface IArgumentTypeDescriptor
    {
        bool CanSupport(Type type);
        string GetDisplayName(IArgument argument);
        object ParseString(IArgument argument, string value);
    }
}