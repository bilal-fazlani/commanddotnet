using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.CommandInvoker;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class CommandRunner
    {
        private readonly CommandLineApplication _app;
        private readonly Type _type;
        private readonly IEnumerable<ArgumentInfo> _constrcutorParamValues;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly AppSettings _appSettings;
        private readonly ModelValidator _modelValidator;
        private readonly ArgumentMerger _argumentMerger;
        private readonly AppInstanceCreator _appInstanceCreator;

        public CommandRunner(
            CommandLineApplication app,
            Type type,
            IEnumerable<ArgumentInfo> constrcutorParamValues,
            IDependencyResolver dependencyResolver,
            AppSettings appSettings)
        {
            _app = app;
            _type = type;
            _constrcutorParamValues = constrcutorParamValues;
            _dependencyResolver = dependencyResolver;
            _appSettings = appSettings;
            _modelValidator = new ModelValidator(dependencyResolver);
            _argumentMerger = new ArgumentMerger(appSettings);
            _appInstanceCreator = new AppInstanceCreator(appSettings);
        }

        public async Task<int> RunCommand(
            CommandInfo commandInfo,
            List<ArgumentInfo> parameterValues)
        {
            parameterValues = parameterValues ?? new List<ArgumentInfo>();

            //get values for method invokation
            object[] mergedParameters = _argumentMerger.Merge(parameterValues);
                
            //validate all parameters
            foreach (dynamic param in mergedParameters)
            {
                _modelValidator.ValidateModel(param);
            }

            //create instance
            object instance = _appInstanceCreator.CreateInstance(_type, _constrcutorParamValues, _dependencyResolver, _modelValidator);

            CommandInvocation commandInvocation = new CommandInvocation
            {
                CommandInfo = commandInfo,
                MergedParameters = mergedParameters,
                Instance = instance,
                ParameterValues = parameterValues
            };

            object returnedObject = _appSettings.CommandInvoker.Invoke(commandInvocation);

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
    }
}