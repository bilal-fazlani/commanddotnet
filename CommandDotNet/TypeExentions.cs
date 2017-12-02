using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public static class TypeExentions
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider attributeProvider)
        {
            T attribute = attributeProvider
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .SingleOrDefault();
            
            return attribute; 
        }
    }
}