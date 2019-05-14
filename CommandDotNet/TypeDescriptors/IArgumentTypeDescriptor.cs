using System;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public interface IArgumentTypeDescriptor
    {
        bool CanSupport(Type type);
        string GetDisplayName(ArgumentInfo argumentInfo);
        object ParseString(ArgumentInfo argumentInfo, string value);
    }
}