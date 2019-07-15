using System;
using System.Collections.Generic;
using CommandDotNet.Builders;

namespace CommandDotNet.Tests.Utils
{
    public class TestDependencyResolver : IDependencyResolver
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        public object Resolve(Type type)
        {
            return _services[type];
        }

        public bool TryResolve(Type type, out object item)
        {
            return _services.TryGetValue(type, out item);
        }

        public void Register(object service)
        {
            // don't allow accidental overwrite
            _services.Add(service.GetType(), service);
        }
    }
}