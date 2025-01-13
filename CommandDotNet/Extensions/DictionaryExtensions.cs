using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Extensions;

internal static class DictionaryExtensions
{
    internal static TValue? GetValueOrDefault<TValue>(this IDictionary dictionary, string key) =>
        dictionary.Contains(key) ? (TValue)dictionary[key]! : default;

    internal static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey,TValue> createDefault)
    {
        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            dictionary[key] = value = createDefault(key);
        }

        return value;
    }

    internal static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue?>? factory = null)
        where TValue : class =>
        dictionary.ThrowIfNull().TryGetValue(key, out var value)
            ? value
            : factory?.Invoke();
}