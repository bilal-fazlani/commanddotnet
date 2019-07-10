using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;

namespace CommandDotNet.Invocation
{
    internal class CommandRunner
    {
        private readonly Type _type;
        private readonly IEnumerable<ArgumentInfo> _constructorParamValues;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly AppSettings _appSettings;
        private readonly ModelValidator _modelValidator;
        private readonly ArgumentMerger _argumentMerger;
        private readonly AppInstanceCreator _appInstanceCreator;

        public CommandRunner(
            Type type,
            IEnumerable<ArgumentInfo> constructorParamValues,
            IDependencyResolver dependencyResolver,
            AppSettings appSettings)
        {
            _type = type;
            _constructorParamValues = constructorParamValues;
            _dependencyResolver = dependencyResolver;
            _appSettings = appSettings;
            _modelValidator = new ModelValidator(dependencyResolver);
            _argumentMerger = new ArgumentMerger(appSettings);
            _appInstanceCreator = new AppInstanceCreator(appSettings);
        }


        internal static int Execute(ExecutionContext executionContext, Func<ExecutionContext, int> next)
        {
            var command = executionContext.ParseResult.Command;
            var commandInfo = command.ContextData.Get<CommandInfo>();
            var commandRunner = command.ContextData.Get<CommandRunner>();

            return commandRunner.RunCommand(commandInfo).Result;
        }

        private async Task<int> RunCommand(CommandInfo commandInfo)
        {
            var argumentInfos = commandInfo.Arguments.ToList();

            //get values for method invocation
            object[] mergedParameters = _argumentMerger.Merge(argumentInfos);
                
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
                ArgsFromCli = argumentInfos
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