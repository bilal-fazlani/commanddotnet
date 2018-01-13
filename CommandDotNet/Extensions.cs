using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public static class Extensions
    {
        public static IEnumerable<CommandInfo> GetCommandInfos(this Type type, AppSettings settings)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => !m.HasAttribute<DefaultMethodAttribute>())
                .Select(mi => new CommandInfo(mi, settings));
        }

        public static CommandInfo GetDefaultCommandInfo(this Type type, AppSettings settings)
        {
            CommandInfo defaultCommandInfo = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.HasAttribute<DefaultMethodAttribute>())
                .Select(mi => new CommandInfo(mi, settings))
                .FirstOrDefault();
            
            return defaultCommandInfo;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            T attribute = (T)attributeProvider.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            return attribute != null;
        }
        
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            T attribute = (T)attributeProvider.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            return attribute;
        }

        public static bool IsCollection(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public static object GetDefaultValue(this PropertyInfo propertyInfo)
        {
            object instance = Activator.CreateInstance(propertyInfo.DeclaringType);
            object defaultValue = propertyInfo.GetValue(instance);
            if (object.Equals(propertyInfo.PropertyType.GetDefaultValue(), defaultValue))
            {
                return DBNull.Value;
            }

            return defaultValue;
        }
        
        public static object GetDefaultValue(this Type type)
        {
            Func<object> f = GetDefault<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }
        
        public static bool IsCompilerGenerated(this Type t) {
            if (t == null)
                return false;

            return t.IsDefined(typeof(CompilerGeneratedAttribute), false)
                   || IsCompilerGenerated(t.DeclaringType);
        }

        public static IEnumerable<PropertyInfo> GetDeclaredProperties<TAttribute>(this Type type) where TAttribute: Attribute
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.HasAttribute<TAttribute>());
        }
        
        public static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.IsSpecialName)
                .Where(x => !x.PropertyType.IsCompilerGenerated());
        }
    }
}