using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
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

        public static void CreateSubApplications(this Type type, 
            AppSettings settings, 
            CommandLineApplication parentApplication)
        {
            IEnumerable<Type> propertySubmodules = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.PropertyType.IsValueType)
                .Select(p => p.PropertyType);

            IEnumerable<Type> inlineClassSubmodules = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);            
            
            foreach (Type submoduleType in propertySubmodules.Union(inlineClassSubmodules))
            {
                AppCreator appCreator = new AppCreator(settings);
                appCreator.CreateApplication(submoduleType, parentApplication);
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="attribute"></param>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <returns></returns>
        public static bool HasAttribute<T>(this ParameterInfo parameterInfo, out T attribute) where T : Attribute
        {
            attribute = parameterInfo.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        public static bool HasAttribute<T>(this ParameterInfo parameterInfo) where T : Attribute
        {
            T attribute = parameterInfo.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="attribute"></param>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <returns></returns>
        public static bool HasAttribute<T>(this MethodInfo methodInfo, out T attribute) where T : Attribute
        {
            attribute = methodInfo.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        public static bool HasAttribute<T>(this MethodInfo methodInfo) where T : Attribute
        {
            T attribute = methodInfo.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <typeparam name="T">Attribute Type</typeparam>
        /// <returns></returns>
        public static bool HasAttribute<T>(this Type type, out T attribute) where T : Attribute
        {
            attribute = type.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
           T attribute = type.GetCustomAttribute<T>();
            return attribute != null;
        }

        public static bool IsCollection(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }
    }
}