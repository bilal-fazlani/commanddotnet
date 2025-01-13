using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution;

public class Services(IServices? appConfigServices = null) : IServices
{
    private readonly Dictionary<Type, object> _servicesByType = new();

    public void Add<T>(T value) where T : class => Add(typeof(T), value);

    public void Add(Type type, object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!type.IsInstanceOfType(value))
        {
            throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
        }
            
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryGetValue
        if (_servicesByType.ContainsKey(type))
        {
            throw new ArgumentException($"service for {nameof(type)} '{type}' already exists '{_servicesByType[type]}'");
        }

        _servicesByType.Add(type, value);
    }

    public void AddOrUpdate<T>(T value) where T : class => AddOrUpdate(typeof(T), value);

    public void AddOrUpdate(Type type, object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!type.IsInstanceOfType(value))
        {
            throw new ArgumentException($"{nameof(value)} '{value.GetType()}' must be an instance of {nameof(type)} '{type}'");
        }

        _servicesByType[type] = value;
    }

    public T GetOrCreate<T>() where T : class, new() => (T)GetOrAdd(typeof(T), () => new T());

    public T GetOrAdd<T>(Func<T> factory) where T : class => (T) GetOrAdd(typeof(T), factory);

    public object GetOrAdd(Type type, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

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

    public T? GetOrDefault<T>() where T : class => (T?)GetOrDefault(typeof(T));

    public object? GetOrDefault(Type type) =>
        _servicesByType.TryGetValue(type, out var value)
            ? value
            : appConfigServices?.GetOrDefault(type);

    public T GetOrThrow<T>() where T : class => (T)GetOrThrow(typeof(T));

    public object GetOrThrow(Type type) =>
        GetOrDefault(type) 
        ?? throw new ArgumentOutOfRangeException($"no service exists for type '{type}'");

    public ICollection<KeyValuePair<Type, object>> GetAll() =>
        appConfigServices is null
            ? _servicesByType.ToCollection()
            : _servicesByType
                .Concat(appConfigServices.GetAll())
                .GroupBy(kvp => kvp.Key)
                .Select(group => group.First()) // retain the first occurrence
                .ToCollection();

    public bool Contains<T>() where T : class => Contains(typeof(T));

    public bool Contains(Type type) =>
        _servicesByType.ContainsKey(type)
        || (appConfigServices?.Contains(type) ?? false);
}