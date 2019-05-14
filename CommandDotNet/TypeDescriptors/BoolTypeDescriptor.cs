using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class BoolTypeDescriptor : 
        IArgumentTypeDescriptor, 
        IAllowedValuesTypeDescriptor
    {

        public bool CanSupport(Type type)
        {
            return type == typeof(bool);
        }
        
        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return argumentInfo.IsImplicit
                ? Constants.TypeDisplayNames.Flag
                : Constants.TypeDisplayNames.Boolean;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            return argumentInfo.IsImplicit
                ? value == "on"
                : bool.Parse(value);
        }

        public IEnumerable<string> GetAllowedValues(ArgumentInfo argumentInfo)
        {
            return argumentInfo.IsImplicit
                ? Enumerable.Empty<string>()
                : new List<string>() {"true", "false"};
        }
    }
}