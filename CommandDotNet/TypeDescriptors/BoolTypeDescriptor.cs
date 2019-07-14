using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public string GetDisplayName(IArgument argument)
        {
            return argument.Arity.AllowsNone()
                ? Constants.TypeDisplayNames.Flag
                : Constants.TypeDisplayNames.Boolean;
        }

        public object ParseString(IArgument argument, string value)
        {
            return bool.Parse(value);
        }

        public IEnumerable<string> GetAllowedValues(IArgument argument)
        {
            return argument.Arity.AllowsNone()
                ? Enumerable.Empty<string>()
                : new List<string> { "true", "false" };
        }
    }
}