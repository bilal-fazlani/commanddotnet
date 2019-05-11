using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Models;

namespace CommandDotNet.Extensions
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<CommandInfo> GetCommandInfos(this Type type, AppSettings settings)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => !m.HasAttribute<DefaultMethodAttribute>())
                .Where(m => (typeof(IDisposable).IsAssignableFrom(type) && m.Name == "Dispose") ? m.Name != "Dispose" : true )
                .Select(mi => new CommandInfo(mi, settings));
        }

        internal static CommandInfo GetDefaultCommandInfo(this Type type, AppSettings settings)
        {
            CommandInfo defaultCommandInfo = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.HasAttribute<DefaultMethodAttribute>())
                .Select(mi => new CommandInfo(mi, settings))
                .FirstOrDefault();
            
            return defaultCommandInfo;
        }

        internal static bool IsCollection(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }
        
        internal static object GetDefaultValue(this Type type)
        {
            Func<object> f = GetDefaultValue<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        }

        private static T GetDefaultValue<T>()
        {
            return default(T);
        }
        
        internal static bool IsCompilerGenerated(this Type t) {
            if (t == null)
                return false;

            return t.IsDefined(typeof(CompilerGeneratedAttribute), false)
                   || IsCompilerGenerated(t.DeclaringType);
        }

        internal static IEnumerable<PropertyInfo> GetDeclaredProperties<TAttribute>(this Type type) where TAttribute: Attribute
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.HasAttribute<TAttribute>());
        }
        
        internal static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.IsSpecialName)
                .Where(x => !x.PropertyType.IsCompilerGenerated());
        }

        internal static bool InheritsFrom<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}