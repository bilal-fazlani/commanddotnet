using System;
using System.Collections.Generic;

namespace CommandDotNet.TypeDescriptors
{
    // begin-snippet: type_descriptors_enum
    public class EnumTypeDescriptor : 
        IArgumentTypeDescriptor,
        IAllowedValuesTypeDescriptor
    {
        public bool CanSupport(Type type) => 
            type.IsEnum;

        public string GetDisplayName(IArgument argument) => 
            argument.TypeInfo.UnderlyingType.Name;

        public object ParseString(IArgument argument, string value) => 
            Enum.Parse(argument.TypeInfo.UnderlyingType, value);

        public IEnumerable<string> GetAllowedValues(IArgument argument) => 
            Enum.GetNames(argument.TypeInfo.UnderlyingType);
    }
    // end-snippet
}