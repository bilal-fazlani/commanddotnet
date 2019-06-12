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

        public void Register(object service)
        {
            // don't allow accidental overwrite
            _services.Add(service.GetType(), service);
        }
    }
}