using System;
using System.Collections.Generic;

namespace CommandDotNet.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createDefault)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                dictionary[key] = value = createDefault();
            }

            return value;
        }
    }
}