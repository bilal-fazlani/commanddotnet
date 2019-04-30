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
            
            return this.GetOrAdd(type, () => this.CreateInstance(type));
        }
        
        public object Resolve(Type type)
        {
            return this.GetOrAdd(type, () => this.CreateInstance(type));
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

        private object CreateInstance(Type type)
        {
            // HostProvidedResolver is allowed first attempt
            // to create/resolve the instance.
            // This let's the app manage the instance lifecycle
            // when desired.
            if (this.TryResolve(type, out var resolve))
            {
                return resolve;
            }

            // If the type is an IArgumentModel, then this framework
            // is responsible for model lifecycle if the HostProvidedResolver
            // is not configured or to do so.
            return IsArgumentModel(type) 
                ? Activator.CreateInstance(type) 
                : null;
        }

        private bool TryResolve(Type type, out object instance)
        {
            instance = null;
            
            if (this.HostProvidedResolver != null)
            {
                try
                {
                    instance = this.HostProvidedResolver.Resolve(type);
                }
                catch (Exception e)
                {
                    // swallow exception.
                    // TODO: remove this try/catch if IDependencyResolver gets TryResolve method
                }
            }

            return instance != null;
        }
    }
}