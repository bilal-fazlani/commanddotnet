using System;
using System.Diagnostics.CodeAnalysis;

namespace CommandDotNet.Builders
{
    public interface IDependencyResolver
    {
        object? Resolve(Type type);
        bool TryResolve(Type type, [NotNullWhen(true)] out object? item);
    }
}