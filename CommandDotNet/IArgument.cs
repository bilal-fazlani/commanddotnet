using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet
{
    public interface IArgument: INameAndDescription
    {
        ITypeInfo TypeInfo { get; }
        IArgumentArity Arity { get; }
        object DefaultValue { get; }
        IReadOnlyCollection<string> AllowedValues { get; set; }
        IReadOnlyCollection<string> Aliases { get; }

        ICustomAttributeProvider CustomAttributes { get; }
        IServices Services { get; }
    }
}