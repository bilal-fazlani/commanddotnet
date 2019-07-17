using System.Collections.Generic;

namespace CommandDotNet.TypeDescriptors
{
    /// <summary>
    ///     Implement for TypeDescriptors to provide a list of values for the help command
    /// </summary>
    public interface IAllowedValuesTypeDescriptor
    {
        IEnumerable<string> GetAllowedValues(IArgument argument);
    }
}