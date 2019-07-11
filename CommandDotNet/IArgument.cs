using System.Collections.Generic;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        ITypeInfo TypeInfo { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        List<string> AllowedValues { get; }
        IEnumerable<string> Aliases { get; }

        IContextData ContextData { get; }
    }
}