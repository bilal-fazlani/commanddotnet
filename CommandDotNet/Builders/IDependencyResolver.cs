using System;

namespace CommandDotNet.Builders
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
    }
}