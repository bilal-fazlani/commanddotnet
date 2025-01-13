using System;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.TypeDescriptors;

[PublicAPI]
public class DelegatedTypeDescriptor<T>(string displayName, Func<string, object> parseValueDelegate)
    : IArgumentTypeDescriptor
{
    private readonly string _displayName = displayName.ThrowIfNull();
    private readonly Func<string, object> _parseValueDelegate = parseValueDelegate.ThrowIfNull();

    public bool CanSupport(Type type) => type == typeof(T);

    public string GetDisplayName(IArgument argument) => _displayName;

    public object ParseString(IArgument argument, string value) => _parseValueDelegate(value);

    public override string ToString() => $"DelegatedTypeDescriptor<{typeof(T).Name}>: '{_displayName}'";
}