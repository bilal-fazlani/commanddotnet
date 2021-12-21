using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.TypeDescriptors
{
    // begin-snippet: type_descriptors_bool
    public class BoolTypeDescriptor : 
        IArgumentTypeDescriptor, 
        IAllowedValuesTypeDescriptor
    {
        public bool CanSupport(Type type) => type == typeof(bool);

        public string GetDisplayName(IArgument argument) =>
            argument.Arity.RequiresNone()
                ? "" // no display name for flags
                : Resources.A.Type_Boolean;

        public object ParseString(IArgument argument, string value) => bool.Parse(value);

        public IEnumerable<string> GetAllowedValues(IArgument argument) =>
            argument.Arity.RequiresNone()
                ? Enumerable.Empty<string>() // no values allowed for flags
                : new List<string> { "true", "false" };
    }
    // end-snippet
}