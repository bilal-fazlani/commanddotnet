using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class AppInstanceCreator
    {
        private readonly ArgumentMerger _argumentMerger;

        public AppInstanceCreator(AppSettings appSettings)
        {
            _argumentMerger = new ArgumentMerger(appSettings);
        }
        
        public object CreateInstance(
            Type type, 
            IEnumerable<ArgumentInfo> constructionParams, 
            IDependencyResolver dependencyResolver,
            ModelValidator modelValidator)
        {
            constructionParams = constructionParams ?? new List<ArgumentInfo>();
            
            //create instance
            object[] mergedValues = _argumentMerger.Merge(constructionParams);
            
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
                else // there are some properties but there is no dependency resolver set
                {
                    throw new AppRunnerException("Dependency resolver is not set for injecting properties. " +
                                                 "Please use an IoC framework'");
                }
            }
            
            return instance;
        }
    }
}