using System;

namespace CommandDotNet
{
    internal static class DependencyResolverExtensions
    {
        internal static bool TryResolve(this IDependencyResolver resolver, Type type, out object item)
        {
            // Some DI frameworks will throw exceptions when attempting to resolve unregistered services

            // These methods should be moved into IDependencyResolver to utilize the DI framework implementations
            // and prevent using exceptions for control flow.

            // This will be a breaking change so we'll need to plan for it.
            try
            {
                item = resolver.Resolve(type);
                return true;
            }
            catch (Exception)
            {
                item = null;
                return false;
            }
        }
    }
}