using System;
using System.Linq;
using System.Reflection;

namespace CommandDotNet.Extensions
{
    internal static class CustomAttributeProviderExtensions
    {
        internal static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            return attributeProvider.IsDefined(typeof(T), true);
        }
        
        internal static T GetCustomAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            T attribute = (T)attributeProvider.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            return attribute;
        }
    }
}