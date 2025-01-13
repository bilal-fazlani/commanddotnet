using System;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>Proxy to get and set the value of an argument</summary>
[PublicAPI]
public class ValueProxy(Func<object?> getter, Action<object?> setter)
{
    /// <summary>The function to get the value</summary>
    public Func<object?> Getter { get; } = getter;

    /// <summary>The function to set the value</summary>
    public Action<object?> Setter { get; } = setter;
}