using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class ArgumentMerger
    {
        private readonly ValueMachine _valueMachine;

        public ArgumentMerger(AppSettings appSettings)
        {
            _valueMachine = new ValueMachine(appSettings);
        }
        
        public object[] Merge(IEnumerable<ArgumentInfo> argumentInfos)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            foreach (var argumentInfo in argumentInfos)
            {
                if (argumentInfo.IsPartOfModel)
                {
                    MergeModel(parameters, argumentInfo);
                }
                else
                {
                    parameters.Add(argumentInfo.PropertyOrParameterName, _valueMachine.GetValue(argumentInfo));
                }
            }

            MergeNestedModels(parameters);

            // TODO: this code assumes the order of arguments will not be changed by the dictionary.
            //       dictionaries do not guarantee order.
            
            return parameters.Values.ToArray();
        }

        private void MergeModel(Dictionary<string, object> parameters, ArgumentInfo argumentInfo)
        {
            object instance = parameters.GetOrAdd(
                argumentInfo.ModelType.FullName, 
                //first property of model
                () => Activator.CreateInstance(argumentInfo.ModelType));

            PropertyInfo propertyInfo = argumentInfo.ModelType.GetProperty(argumentInfo.PropertyOrParameterName);
            propertyInfo.SetValue(instance, _valueMachine.GetValue(argumentInfo));
        }

        private static void MergeNestedModels(Dictionary<string, object> parameters)
        {
            // Nested models rely on the concept that an IArgumentModel cannot be
            // referenced in more than one location for any command method,
            // otherwise the options would be duplicated.
            // Given that we can assume that if a model can be nested
            // then it *must* be nested.
            var argumentModels = parameters.Values.OfType<IArgumentModel>().ToList();
            foreach (var model in argumentModels)
            {
                var nestedModelProps = model.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.InheritsFrom<IArgumentModel>());

                foreach (var prop in nestedModelProps)
                {
                    var key = prop.PropertyType.FullName;
                    if (parameters.TryGetValue(key, out var childModel))
                    {
                        prop.SetValue(model, childModel);
                        parameters.Remove(key);
                    }
                }
            }
        }
    }
}