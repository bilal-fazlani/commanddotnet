using System;

namespace CommandDotNet
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
    }
}