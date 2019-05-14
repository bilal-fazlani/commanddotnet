using System;
using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class EnumTypeDesriptor : 
        IArgumentTypeDescriptor,
        IAllowedValuesTypeDescriptor
    {
        public bool CanSupport(Type type)
        {
            return type.IsEnum;
        }
        
        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return argumentInfo.UnderlyingType.Name;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            return Enum.Parse(argumentInfo.UnderlyingType, value);
        }

        public IEnumerable<string> GetAllowedValues(ArgumentInfo argumentInfo)
        {
            return Enum.GetNames(argumentInfo.UnderlyingType);
        }
    }
}