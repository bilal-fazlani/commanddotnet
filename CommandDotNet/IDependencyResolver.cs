using System;

namespace CommandDotNet
{
    public interface IDependencyResolver
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}