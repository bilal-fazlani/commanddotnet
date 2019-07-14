using System;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IArgumentDef
    {
        ArgumentType ArgumentType { get; }
        string Name { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object DefaultValue { get; }
        ICustomAttributeProvider Attributes { get; }
        void SetValue(object value);
        IArgument Argument { get; set; }
    }
}