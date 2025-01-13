using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace CommandDotNet.TypeDescriptors;

[PublicAPI]
// begin-snippet: type_descriptors_type_convertor
public class ComponentModelTypeDescriptor : IArgumentTypeDescriptor
{
    public bool CanSupport(Type type) => 
        TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));

    public string GetDisplayName(IArgument argument) => 
        argument.TypeInfo.UnderlyingType.Name;

    public object ParseString(IArgument argument, string value)
    {
        var typeConverter = argument.Arity.AllowsMany()
            ? TypeDescriptor.GetConverter(argument.TypeInfo.UnderlyingType)
            : TypeDescriptor.GetConverter(argument.TypeInfo.Type);
        return typeConverter.ConvertFrom(value)!;
    }
    // end-snippet
}