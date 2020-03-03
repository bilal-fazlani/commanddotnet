using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    public class Services : IServices
    {
        private readonly Dictionary<Type, object> _servicesByType = new Dictionary<Type, object>();

        public void Add<T>(T value)
        {
            Add(typeof(T), value);
        }

        public void Add(Type type, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if(!type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
            }
            if (_servicesByType.ContainsKey(type))
            {
                throw new ArgumentException($"service for {nameof(type)} '{type}' already exists '{_servicesByType[type]}'");
            }

            _servicesByType.Add(type, value);
        }

        public void AddOrUpdate<T>(T value)
        {
            AddOrUpdate(typeof(T), value);
        }

        public void AddOrUpdate(Type type, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
            }

            _servicesByType[type] = value;
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public object Get(Type type)
        {
            return _servicesByType.GetValueOrDefault(type);
        }

        public ICollection<KeyValuePair<Type, object>> GetAll()
        {
            return _servicesByType.ToCollection();
        }
    }
}