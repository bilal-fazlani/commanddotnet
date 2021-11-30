using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    public class Services : IServices
    {
        private readonly IServices? _appConfigServices;
        private readonly Dictionary<Type, object> _servicesByType = new();

        public Services(IServices? appConfigServices = null)
        {
            _appConfigServices = appConfigServices;
        }

        public void Add<T>(T value) where T : class
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

        public void AddOrUpdate<T>(T value) where T : class
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

        public T GetOrCreate<T>() where T : class, new()
        {
            return (T)GetOrAdd(typeof(T), () => new T());
        }

        public T GetOrAdd<T>(Func<T> factory) where T : class
        {
            return (T) GetOrAdd(typeof(T), factory);
        }

        public object GetOrAdd(Type type, Func<object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var value = GetOrDefault(type);
            if (value == null)
            {
                value = factory();
                if (value == null)
                {
                    throw new InvalidConfigurationException($"'{factory}' returned null");
                }
                Add(type, value);
            }

            return value;
        }

        public T? GetOrDefault<T>() where T : class
        {
            return (T?)GetOrDefault(typeof(T));
        }

        public object? GetOrDefault(Type type)
        {
            return _servicesByType.TryGetValue(type, out var value)
                ? value
                : _appConfigServices?.GetOrDefault(type);
        }

        public T GetOrThrow<T>() where T : class
        {
            return (T)GetOrThrow(typeof(T));
        }

        public object GetOrThrow(Type type)
        {
            return GetOrDefault(type) 
                   ?? throw new ArgumentOutOfRangeException($"no service exists for type '{type}'");
        }

        public ICollection<KeyValuePair<Type, object>> GetAll()
        {
            return _appConfigServices is null
                ? _servicesByType.ToCollection()
                : _servicesByType
                    .Union(_appConfigServices.GetAll())
                    .ToCollection();
        }

        public bool Contains<T>() where T : class
        {
            return Contains(typeof(T));
        }

        public bool Contains(Type type)
        {
            return _servicesByType.ContainsKey(type)
                   || (_appConfigServices?.Contains(type) ?? false);
        }
    }
}