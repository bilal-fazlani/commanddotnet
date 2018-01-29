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
    internal class CommandRunner
    {
        private readonly CommandLineApplication _app;
        private readonly Type _type;
        private readonly IEnumerable<ArgumentInfo> _constrcutorParamValues;
        private readonly IDependencyResolver _dependencyResolver;
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
            _modelValidator = new ModelValidator(dependencyResolver);
            _argumentMerger = new ArgumentMerger(appSettings);
            _appInstanceCreator = new AppInstanceCreator(appSettings);
        }

        public async Task<int> RunCommand(
            CommandInfo commandInfo,
            List<ArgumentInfo> parameterValues)
        {
            parameterValues = parameterValues ?? new List<ArgumentInfo>();

            try
            {
                //create instance
                object instance = _appInstanceCreator.CreateInstance(_type, _constrcutorParamValues, _dependencyResolver, _modelValidator);

                //dentify method to invove
                MethodInfo theMethod = instance.GetType().GetMethod(commandInfo.MethodName);

                //get values for method invokation
                object[] mergedParameters = _argumentMerger.Merge(parameterValues);
                
                //validate all parameters
                foreach (dynamic param in mergedParameters)
                {
                    _modelValidator.ValidateModel(param);
                }
                
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
    }
}