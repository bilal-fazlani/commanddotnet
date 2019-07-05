using System;
using CommandDotNet.ClassModeling;

namespace CommandDotNet.TypeDescriptors
{
    public interface IArgumentTypeDescriptor
    {
        bool CanSupport(Type type);
        string GetDisplayName(ArgumentInfo argumentInfo);
        object ParseString(ArgumentInfo argumentInfo, string value);
    }
}