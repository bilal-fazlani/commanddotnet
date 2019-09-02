using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    /// <summary>An argument is either an <see cref="Option"/> or <see cref="Operand"/></summary>
    public interface IArgument: INameAndDescription
    {
        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        ITypeInfo TypeInfo { get; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        IArgumentArity Arity { get; }

        /// <summary>The default value for this argument</summary>
        object DefaultValue { get; set; }

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        IReadOnlyCollection<string> AllowedValues { get; set; }

        /// <summary>The aliases defined for this argument</summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>The attributes defined on the parameter or property that define this argument</summary>
        ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware and associated with this argument</summary>
        IServices Services { get; }
    }
}