using System.Collections.Generic;

namespace CommandDotNet.Builders.ArgumentDefaults
{
    public delegate IEnumerable<string> GetArgumentKeysDelegate(IArgument argument);
}