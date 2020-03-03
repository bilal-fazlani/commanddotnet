using System.Collections.Generic;

namespace CommandDotNet.Extensions
{
    internal static class CollectionExtensions
    {
        internal static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) =>
            items.ForEach(collection.Add);
    }
}