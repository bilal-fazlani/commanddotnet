using System;

namespace CommandDotNet.TypeDescriptors
{
    public interface IArgumentTypeDescriptor
    {
        /// <summary>Returns true when the type can be described by this descriptor</summary>
        bool CanSupport(Type type);

        /// <summary>Returns the name that will be displayed in help documentation</summary>
        string GetDisplayName(IArgument argument);

        /// <summary>Parses the string</summary>
        object? ParseString(IArgument argument, string value);
    }
}