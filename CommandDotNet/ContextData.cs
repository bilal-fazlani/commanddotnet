using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    internal class ContextData : IContextData
    {
        private readonly Dictionary<Type, object> _servicesByType = new Dictionary<Type, object>();

        public void Add<T>(T value)
        {
            _servicesByType.Add(typeof(T), value);
        }

        public void Set<T>(T value)
        {
            _servicesByType[typeof(T)] = value;
        }

        public object Get(Type type)
        {
            return _servicesByType.GetValueOrDefault(type);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }
    }
}