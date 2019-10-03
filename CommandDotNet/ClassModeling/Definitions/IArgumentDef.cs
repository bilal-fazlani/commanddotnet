using System;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IArgumentDef: ISourceDef
    {
        CommandNodeType CommandNodeType { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object DefaultValue { get; }
        IArgument Argument { get; set; }
        void SetValue(object value);
    }
}