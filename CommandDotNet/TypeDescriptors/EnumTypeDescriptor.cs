using System;
using System.Collections.Generic;

namespace CommandDotNet.TypeDescriptors
{
    public class EnumTypeDescriptor : 
        IArgumentTypeDescriptor,
        IAllowedValuesTypeDescriptor
    {
        public bool CanSupport(Type type)
        {
            return type.IsEnum;
        }

        public string GetDisplayName(IArgument argument)
        {
            return argument.TypeInfo.UnderlyingType.Name;
        }

        public object ParseString(IArgument argument, string value)
        {
            return Enum.Parse(argument.TypeInfo.UnderlyingType, value);
        }

        public IEnumerable<string> GetAllowedValues(IArgument argument)
        {
            return Enum.GetNames(argument.TypeInfo.UnderlyingType);
        }
    }
}