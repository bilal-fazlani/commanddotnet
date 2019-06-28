using System;
using System.Collections.Generic;

namespace CommandDotNet.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey,TValue> createDefault)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                dictionary[key] = value = createDefault(key);
            }

            return value;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return !dictionary.TryGetValue(key, out var obj) 
                ? defaultValue 
                : obj;
        }
    }
}