using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        ITypeInfo TypeInfo { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        List<string> AllowedValues { get; set; }
        IEnumerable<string> Aliases { get; }

        ICustomAttributeProvider CustomAttributes { get; }
        IContextData ContextData { get; }
    }
}