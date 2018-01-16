using System;
using System.Linq;
using System.Reflection;

namespace CommandDotNet.Extensions
{
    public static class CustomAttributeProviderExtensions
    {
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
    }
}