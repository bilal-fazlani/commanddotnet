using System;

namespace CommandDotNet.Execution
{
    public class ResolverReturnedNullException : Exception
    {
        public ResolverReturnedNullException(Type type)
            :base($"The resolver returned null for type '{type}'. The type may not be registered.")
        {
        }
    }
}