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
        private readonly IEnumerable<ArgumentInfo> _constructorParamValues;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly AppSettings _appSettings;
        private readonly ModelValidator _modelValidator;
        private readonly ArgumentMerger _argumentMerger;
        private readonly AppInstanceCreator _appInstanceCreator;

        public CommandRunner(
            CommandLineApplication app,
            Type type,
            IEnumerable<ArgumentInfo> constructorParamValues,
            IDependencyResolver dependencyResolver,
            AppSettings appSettings)
        {
            _app = app;
            _type = type;
            _constructorParamValues = constructorParamValues;
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
            object instance = _appInstanceCreator.CreateInstance(_type, _constructorParamValues, _dependencyResolver, _modelValidator);
            
            CommandInvocation commandInvocation = new CommandInvocation
            {
                CommandInfo = commandInfo,
                ParamsForCommandMethod = mergedParameters,
                Instance = instance,
                ArgsFromCli = parameterValues
            };

            object returnedObject = _appSettings.CommandInvoker.Invoke(commandInvocation);

            //disposing --
            switch (instance)
            {
                case IDisposable disposable : disposable.Dispose();
                    break;
            }
            //disposing --
            
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