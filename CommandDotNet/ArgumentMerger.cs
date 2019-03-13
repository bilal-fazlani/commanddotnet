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
        private readonly AppSettings _appSettings;
        private readonly ValueMachine _valueMachine;

        public ArgumentMerger(AppSettings appSettings)
        {
            _appSettings = appSettings;
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
                    parameters.Add(argumentInfo.PropertyOrArgumentName, _valueMachine.GetValue(argumentInfo));
                }
            }
            
            return parameters.Values.ToArray();
        }

        private void MergeModel(Dictionary<string, object> parameters, ArgumentInfo argumentInfo)
        {
            object instance = parameters.GetOrAdd(
                argumentInfo.ModelType.FullName, 
                //first property of model
                () => Activator.CreateInstance(argumentInfo.ModelType));

            PropertyInfo propertyInfo = argumentInfo.ModelType.GetProperty(argumentInfo.PropertyOrArgumentName);
            propertyInfo.SetValue(instance, _valueMachine.GetValue(argumentInfo));
        }
    }
}