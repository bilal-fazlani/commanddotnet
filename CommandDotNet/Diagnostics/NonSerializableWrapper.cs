using System;
using System.Runtime.Serialization;
using CommandDotNet.Extensions;

namespace CommandDotNet.Diagnostics;

/// <summary>
/// A NonSerializableWrapper is a wrapper to contain non-serializable objects
/// within Exception.Data which requires all items to be serializable.
/// This is required for older versions of the dotnet.
/// </summary>
[Serializable]
internal class NonSerializableWrapper(object? item, bool skipPrint = false) : ISerializable, IIndentableToString
{
    public object? Item { get; } = item;
    public bool SkipPrint { get; } = skipPrint;

    public T As<T>() => (T)Item! ?? throw new InvalidConfigurationException($"{nameof(NonSerializableWrapper)}.{nameof(Item)} cannot be null when using As<T>");

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => 
        info.AddValue(Item?.GetType().Namespace ?? "???", Item?.ToString());

    public string ToString(Indent indent) => Item.ToIndentedString(indent) ?? "";
}