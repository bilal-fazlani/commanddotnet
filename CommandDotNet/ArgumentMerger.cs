using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                if (!argumentInfo.IsPartOfModel)
                    parameters.Add(argumentInfo.PropertyOrArgumentName, _valueMachine.GetValue(argumentInfo));
                
                else
                {
                    object instance;
                    if (parameters.ContainsKey(argumentInfo.ModelType.FullName)) //already added one or more params
                    {
                        instance = parameters[argumentInfo.ModelType.FullName];
                    }
                    else //first property of model
                    {
                        instance = Activator.CreateInstance(argumentInfo.ModelType);
                        parameters.Add(argumentInfo.ModelType.FullName, instance);
                    }

                    PropertyInfo propertyInfo = argumentInfo.ModelType.GetProperty(argumentInfo.PropertyOrArgumentName);
                    propertyInfo.SetValue(instance, _valueMachine.GetValue(argumentInfo));
                }
            }
            
            return parameters.Values.ToArray();
        }
    }
}