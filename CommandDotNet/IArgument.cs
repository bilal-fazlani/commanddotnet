using System.Collections.Generic;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        string TypeDisplayName { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        List<string> AllowedValues { get; }
        List<string> Values { get; }
        IEnumerable<string> Aliases { get; }
    }
}