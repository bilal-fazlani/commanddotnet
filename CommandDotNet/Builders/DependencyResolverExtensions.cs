using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CommandDotNet.Builders;

[PublicAPI]
public static class DependencyResolverExtensions
{
    public static T? Resolve<T>(this IDependencyResolver resolver) where T : class? 
        => (T?)resolver.Resolve(typeof(T));

    public static bool TryResolve<T>(this IDependencyResolver resolver, 
        [NotNullWhen(true)] out T? item)
        where T : class?
    {
        var result = resolver.TryResolve(typeof(T), out var localItem);
        item = (T?) localItem;
        return result;
    }

    public static T? ResolveOrDefault<T>(this IDependencyResolver resolver) where T: class? 
        => resolver.TryResolve(out T? item) ? item : null;
}