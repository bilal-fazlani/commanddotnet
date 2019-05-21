using System;
using System.ComponentModel;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class ComponentModelTypeDescriptor : IArgumentTypeDescriptor
    {
        public bool CanSupport(Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            return typeConverter.CanConvertFrom(typeof(string));
        }
     
        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return argumentInfo.UnderlyingType.Name;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            var typeConverter = argumentInfo.IsMultipleType
                ? TypeDescriptor.GetConverter(argumentInfo.UnderlyingType)
                : TypeDescriptor.GetConverter(argumentInfo.Type);
            return typeConverter.ConvertFrom(value);
        }
    }
}