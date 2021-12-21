using System;
using System.ComponentModel;

namespace CommandDotNet.TypeDescriptors
{
    // begin-snippet: type_descriptors_type_convertor
    public class ComponentModelTypeDescriptor : IArgumentTypeDescriptor
    {
        public bool CanSupport(Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            return typeConverter.CanConvertFrom(typeof(string));
        }

        public string GetDisplayName(IArgument argument)
        {
            return argument.TypeInfo.UnderlyingType.Name;
        }

        public object? ParseString(IArgument argument, string value)
        {
            var typeConverter = argument.Arity.AllowsMany()
                ? TypeDescriptor.GetConverter(argument.TypeInfo.UnderlyingType)
                : TypeDescriptor.GetConverter(argument.TypeInfo.Type);
            return typeConverter.ConvertFrom(value)!;
        }
        // end-snippet
    }
}