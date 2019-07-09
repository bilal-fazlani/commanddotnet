using System.Collections.Generic;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        string TypeDisplayName { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        List<string> AllowedValues { get; }
        IEnumerable<string> Aliases { get; }

        IContextData ContextData { get; }
    }
}