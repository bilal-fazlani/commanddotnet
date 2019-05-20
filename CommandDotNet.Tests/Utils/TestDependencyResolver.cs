using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.Utils
{
    public class TestDependencyResolver : IDependencyResolver
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        public object Resolve(Type type)
        {
            return _services[type];
        }

        public void Register<T>(T service)
        {
            // don't allow accidental overwrite
            _services.Add(typeof(T), service);
        }
    }
}