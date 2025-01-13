using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>TypeInfo is used to determine how to parse inputs and display help</summary>
[PublicAPI]
public class TypeInfo(Type type, Type underlyingType, string? displayName = null) : ITypeInfo
{
    /// <summary>A bool with no display name</summary>
    public static readonly TypeInfo Flag = Single<bool>();
         
    public static TypeInfo Single<T>(string? displayName = null) => new(typeof(T), typeof(T), displayName);
    public static TypeInfo Enumerable<T>(string? displayName = null) => new(typeof(IEnumerable<T>), typeof(T), displayName);
    public static TypeInfo List<T>(string? displayName = null) => new(typeof(List<T>), typeof(T), displayName);
    public static TypeInfo Array<T>(string? displayName = null) => new(typeof(T[]), typeof(T), displayName);

    /// <summary>The type of the property or parameter defining an argument</summary>
    public Type Type { get; } = type.ThrowIfNull();

    /// <summary>
    /// When <see cref="Type"/> is a generic type (Nullable or IEnumerable) then <see cref="UnderlyingType"/>
    /// is the first generic argument of that type.
    /// </summary>
    public Type UnderlyingType { get; } = underlyingType.ThrowIfNull();

    /// <summary>The name to display in help</summary>
    public string? DisplayName { get; set; } = displayName;
}