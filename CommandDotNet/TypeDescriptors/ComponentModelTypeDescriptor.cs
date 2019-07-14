using System;
using System.ComponentModel;
using CommandDotNet.ClassModeling;

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
            var typeConverter = argumentInfo.Arity.AllowsZeroOrMore()
                ? TypeDescriptor.GetConverter(argumentInfo.UnderlyingType)
                : TypeDescriptor.GetConverter(argumentInfo.Type);
            return typeConverter.ConvertFrom(value);
        }

        public string GetDisplayName(IArgument argument)
        {
            return argument.TypeInfo.UnderlyingType.Name;
        }

        public object ParseString(IArgument argument, string value)
        {
            var typeConverter = argument.Arity.AllowsZeroOrMore()
                ? TypeDescriptor.GetConverter(argument.TypeInfo.UnderlyingType)
                : TypeDescriptor.GetConverter(argument.TypeInfo.Type);
            return typeConverter.ConvertFrom(value);
        }
    }
}