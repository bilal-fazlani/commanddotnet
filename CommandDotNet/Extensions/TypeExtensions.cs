using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CommandDotNet.Extensions
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<MethodInfo> GetCommandMethods(this Type type, bool includeFromBaseClasses)
        {
            var bindingFlags = includeFromBaseClasses
                ? BindingFlags.Public | BindingFlags.Instance
                : BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            return type.GetMethods(bindingFlags)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.DeclaringType != typeof(object))
                .Where(m => !typeof(IDisposable).IsAssignableFrom(type) || m.Name != nameof(IDisposable.Dispose));
        }

        private static bool IsNullableType(this Type type) => Nullable.GetUnderlyingType(type) != null;

        internal static bool IsNullableProperty(this PropertyInfo propertyInfo) =>
            propertyInfo.PropertyType.IsNullableType()
            || (propertyInfo.GetNullability() == NullabilityState.Nullable);

        internal static bool IsNullableParameter(this ParameterInfo parameterInfo) => 
            parameterInfo.ParameterType.IsNullableType() 
            || (parameterInfo.GetNullability() == NullabilityState.Nullable);

        internal static Type GetUnderlyingType(this Type type) =>
            Nullable.GetUnderlyingType(type)
            ?? type.GetListUnderlyingType()
            ?? type;

        private static Type? GetListUnderlyingType(this Type type) =>
            type.IsArray
                ? type.GetElementType()
                : typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType
                    ? type.GetGenericArguments().FirstOrDefault()
                    : null;

        internal static bool IsNonStringEnumerable(this Type type) => 
            type != typeof(string) && type.IsEnumerable();

        private static bool IsEnumerable(this Type type) =>
            type.GetInterfaces()
                .Concat(type.ToEnumerable())
                .Any(x => x == typeof(IEnumerable));

        internal static bool IsNonStringCollection(this Type type) =>
            type != typeof(string) && type.IsCollection();

        internal static bool IsCollection(this Type type) =>
            type.GetInterfaces()
                .Concat(type.ToEnumerable())
                .Any(x => x.IsGenericType
                    ? x.GetGenericTypeDefinition() == typeof(ICollection<>)
                    : x == typeof(ICollection));

        internal static bool IsStaticClass(this Type type) => type.IsAbstract && type.IsSealed;

        private static readonly Dictionary<Type, MethodInfo> DefaultMethodByType = new();

        private static object? GetDefaultValue(this Type type) =>
            DefaultMethodByType.GetOrAdd(type, _ =>
            {
                Func<object?> f = GetDefaultValue<object>;
                return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type);
            }).Invoke(null, null);

        internal static bool IsDefaultFor(this object defaultValue, Type type) => 
            Equals(defaultValue, type.GetDefaultValue());

        private static T? GetDefaultValue<T>() => default;

        internal static bool IsCompilerGenerated(this Type? t) =>
            t is not null
            && (t.IsDefined(typeof(CompilerGeneratedAttribute), false)
                || IsCompilerGenerated(t.DeclaringType));

        internal static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type) =>
            type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.PropertyType.IsCompilerGenerated());

        internal static bool InheritsFrom<T>(this Type type) => 
            typeof(T).IsAssignableFrom(type);
    }
}