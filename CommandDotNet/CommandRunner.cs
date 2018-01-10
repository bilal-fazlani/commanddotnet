using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class CommandRunner
    {
        private readonly CommandLineApplication _app;
        private readonly Type _type;
        private readonly IEnumerable<ArgumentInfo> _constrcutorParamValues;
        private readonly IDependencyResolver _dependencyResolver;

        public CommandRunner(
            CommandLineApplication app,
            Type type,
            IEnumerable<ArgumentInfo> constrcutorParamValues,
            IDependencyResolver dependencyResolver)
        {
            _app = app;
            _type = type;
            _constrcutorParamValues = constrcutorParamValues;
            _dependencyResolver = dependencyResolver;
        }

        public async Task<int> RunCommand(
            CommandInfo commandInfo,
            List<ArgumentInfo> parameterValues)
        {
            parameterValues = parameterValues ?? new List<ArgumentInfo>();

            try
            {
                //create instance
                object instance = AppInstanceCreator.CreateInstance(_type, _constrcutorParamValues, _dependencyResolver);

                //dentify method to invove
                MethodInfo theMethod = instance.GetType().GetMethod(commandInfo.MethodName);

                //get values for method invokation
                object[] mergedParameters = Merge(parameterValues);
                
                //invoke method
                object returnedObject = theMethod.Invoke(instance, mergedParameters);

                //default return code for cases when method is of type void instead of int
                int returnCode = 0;

                //wait for method to complete
                switch (returnedObject)
                {
                    case Task<int> intPromise:
                        returnCode = await intPromise;
                        break;
                    case Task promise:
                        await promise;
                        break;
                    case int intValue:
                        returnCode = intValue;
                        break;
                    //for void and every other return type, the value is already set to 0
                }
                
                //return the actual return code
                return returnCode;
            }
            catch (ValueParsingException e)
            {
                throw new CommandParsingException(_app, e.Message);
            }
        }

        private object[] Merge(List<ArgumentInfo> argumentInfos)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            foreach (var argumentInfo in argumentInfos)
            {
                if (!argumentInfo.IsPartOfModel)
                    parameters.Add(argumentInfo.PropertyOrArgumentName, ValueMachine.GetValue(argumentInfo));
                
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
                    propertyInfo.SetValue(instance, ValueMachine.GetValue(argumentInfo));
                }
            }
            
            return parameters.Values.ToArray();
        }
    }
}