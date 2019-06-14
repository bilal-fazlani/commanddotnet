using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
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
            InjectProperties(instance, dependencyResolver);

            InjectPipedInput(instance);

            return instance;
        }

        private void InjectPipedInput(object instance)
        {
            var property = instance.GetType()
                .GetDeclaredProperties<PipedInputAttribute>()
                .FirstOrDefault();

            if (property == null)
            {
                return;
            }

            var isAssignableFromList = property.PropertyType.IsAssignableFrom(typeof(List<string>));
            var isAssignableFromArray = property.PropertyType.IsAssignableFrom(typeof(string[]));

            if (!isAssignableFromList && !isAssignableFromArray)
            {
                throw new AppRunnerException($"{property.DeclaringType.Name}.{property.Name} must be assignable from either List<string> or string[]");
            }
            
            var keepEmptyLines = property.GetCustomAttribute<PipedInputAttribute>()?.KeepEmptyLines ?? false;
            var pipedInput = PipedInput.GetPipedInput(keepEmptyLines);

            if (pipedInput.InputWasPiped)
            {
                if (isAssignableFromList)
                {
                    property.SetValue(instance, pipedInput.Values.ToList());
                }
                else if (isAssignableFromArray)
                {
                    property.SetValue(instance, pipedInput.Values.ToArray());
                }
            }
        }

        private static void InjectProperties(object instance, IDependencyResolver dependencyResolver)
        {
            List<PropertyInfo> properties = instance.GetType().GetDeclaredProperties<InjectPropertyAttribute>().ToList();

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
        }
    }
}