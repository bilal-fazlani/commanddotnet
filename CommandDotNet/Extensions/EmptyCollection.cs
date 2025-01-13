using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Extensions;

internal class EmptyCollection<T> : IReadOnlyCollection<T>, ICollection<T>
{
    internal static readonly EmptyCollection<T> Instance = new();

    public int Count { get; } = 0;
    public bool IsReadOnly { get; } = true;
    public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(T item) => throw new NotImplementedException();
    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    public bool Remove(T item) => throw new NotImplementedException();
}