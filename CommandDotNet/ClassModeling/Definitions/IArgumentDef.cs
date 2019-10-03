using System;
using CommandDotNet.Builders;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IArgumentDef: ICustomAttributesContainer
    {
        CommandNodeType CommandNodeType { get; }
        string Name { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object DefaultValue { get; }
        IArgument Argument { get; set; }
        void SetValue(object value);
    }
}