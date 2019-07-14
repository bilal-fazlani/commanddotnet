using System.Collections.Generic;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        ITypeInfo TypeInfo { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        List<string> AllowedValues { get; set; }
        IEnumerable<string> Aliases { get; }

        IContextData ContextData { get; }
    }
}