using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public static class AppInstanceCreator
    {                
        public static object CreateInstance(Type type, IEnumerable<ArgumentInfo> construcitonParams, IDependencyResolver dependencyResolver)
        {
            construcitonParams = construcitonParams ?? new List<ArgumentInfo>();
            
            //create instance
            object[] mergedValues = ArgumentMerger.Merge(construcitonParams);
            object instance = Activator.CreateInstance(type, mergedValues);

            //detect properties
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                 //properties which are not subcommands are dependencies
                .Where(p=> p.GetCustomAttribute<SubCommandAttribute>() == null) 
                .ToList();
            
            if (properties.Any())
            {
                if (dependencyResolver != null)
                {
                    foreach (var propertyInfo in properties)
                    {
                        propertyInfo.SetValue(instance, dependencyResolver.Resolve(propertyInfo.PropertyType));
                    }
                }
                else // there are some properties but there is no dependecncy resolver set
                {
                    //todo: show warning or error here
                }
            }
            
            return instance;
        }
    }
}