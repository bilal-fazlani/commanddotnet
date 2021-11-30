using System;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IArgumentDef: ISourceDef
    {
        string ArgumentDefType { get; }
        CommandNodeType CommandNodeType { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object? DefaultValue { get; }
        ValueProxy ValueProxy { get; }
        bool IsOptional { get; }
        BooleanMode? BooleanMode { get; }
        IArgumentArity Arity { get; }
        char? Split { get; set; }
    }
}