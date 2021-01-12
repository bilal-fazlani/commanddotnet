using System;
using System.Runtime.Serialization;

namespace CommandDotNet.Diagnostics
{
    /// <summary>
    /// A NonSerializableWrapper is a wrapper to contain non-serializable objects
    /// within Exception.Data which requires all items to be serializable.
    /// This is required for older versions of the dotnet.
    /// </summary>
    [Serializable]
    internal class NonSerializableWrapper<T> : ISerializable
    {
        public T Item { get; }

        public NonSerializableWrapper(T item)
        {
            Item = item;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => 
            info.AddValue(typeof(T).Name, Item?.ToString());
    }
}