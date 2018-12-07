using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace CommandDotNet
{
    /// <summary>
    /// Caches IArgumentModels for reuse in DependencyInjection
    /// </summary>
    public class CachedArgumentModelResolver : IDependencyResolver
    {
        internal IDependencyResolver HostProvidedResolver { get; set; }
        
        private Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public object ResolveArgumentModel(Type type)
        {
            if (!IsArgumentModel(type))
            {
                return null;
            }
            
            return this.GetOrAdd(type, () => this.CreateInstance(type, false));
        }
        
        public object Resolve(Type type)
        {
            return this.GetOrAdd(type, () => this.CreateInstance(type, true));
        }

        private static bool IsArgumentModel(Type type)
        {
            return typeof(IArgumentModel).IsAssignableFrom(type);
        }

        private object GetOrAdd(Type type, Func<object> create)
        {
            if (!IsArgumentModel(type))
            {
                return create();
            }
            
            if (!this._cache.TryGetValue(type, out var instance))
            {
                this._cache[type] = instance = create();
            }

            return instance;
        }

        private object CreateInstance(Type type, bool tryHostResolverFirst)
        {
            if (tryHostResolverFirst && this.HostProvidedResolver != null)
            {
                try
                {
                    return this.HostProvidedResolver.Resolve(type);
                }
                catch (Exception e)
                {
                    // swallow exception.
                    // TODO: remove this try/catch if IDependencyResolver gets TryResolve method
                }
            }

            return IsArgumentModel(type) 
                ? Activator.CreateInstance(type) 
                : null;
        }
    }
}