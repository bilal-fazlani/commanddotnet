using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Extensions
{
    internal class EmptyCollection<T> : IReadOnlyCollection<T>
    {
        internal static readonly EmptyCollection<T> Instance = new EmptyCollection<T>();

        public int Count { get; } = 0;

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}