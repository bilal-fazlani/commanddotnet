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
        IArgument Argument { get; set; }
        void SetValue(object value);
    }
}