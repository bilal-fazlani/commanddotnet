using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.ClassModeling;

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
            return argumentInfo.Arity.AllowsNone()
                ? Constants.TypeDisplayNames.Flag
                : Constants.TypeDisplayNames.Boolean;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            return bool.Parse(value);
        }

        public IEnumerable<string> GetAllowedValues(ArgumentInfo argumentInfo)
        {
            return argumentInfo.Arity.AllowsNone()
                ? Enumerable.Empty<string>()
                : new List<string> { "true", "false" };
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