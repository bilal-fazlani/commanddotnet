using System;

namespace CommandDotNet.Builders
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
        bool TryResolve(Type type, out object item);
    }

    public static class DependencyResolverExtensions
    {
        public static T Resolve<T>(this IDependencyResolver resolver) => (T)resolver.Resolve(typeof(T));

        public static bool TryResolve<T>(this IDependencyResolver resolver, out T item)
        {
            var result = resolver.TryResolve(typeof(T), out var localItem);
            item = (T)localItem;
            return result;
        }

        public static T ResolveOrDefault<T>(this IDependencyResolver resolver) =>
            resolver.TryResolve<T>(out T item) ? item : default;
    }
}