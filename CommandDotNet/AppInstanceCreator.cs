using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public static class AppInstanceCreator
    {                
        public static object CreateInstance(
            Type type, 
            IEnumerable<ArgumentInfo> construcitonParams, 
            IDependencyResolver dependencyResolver,
            ModelValidator modelValidator)
        {
            construcitonParams = construcitonParams ?? new List<ArgumentInfo>();
            
            //create instance
            object[] mergedValues = ArgumentMerger.Merge(construcitonParams);
            
            //validate all parameters
            foreach (dynamic param in mergedValues)
            {
                modelValidator.ValidateModel(param);
            }
            
            object instance = Activator.CreateInstance(type, mergedValues);

            //detect injection properties
            List<PropertyInfo> properties = type.GetDeclaredProperties<InjectPropertyAttribute>().ToList();
            
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
                    throw new AppRunnerException($"Dependency resolver is not set for injecting properties. " +
                                                 $"Please use an IoC framework'");
                }
            }
            
            return instance;
        }
    }
}