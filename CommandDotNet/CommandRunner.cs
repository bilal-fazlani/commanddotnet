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
        private readonly List<ArgumentInfo> _constrcutorParamValues;
        private readonly AppSettings _appSettings;

        public CommandRunner(
            CommandLineApplication app,
            Type type,
            List<ArgumentInfo> constrcutorParamValues, 
            AppSettings appSettings)
        {
            _app = app;
            _type = type;
            _constrcutorParamValues = constrcutorParamValues;
            _appSettings = appSettings;
        }

        public async Task<int> RunCommand(
            CommandInfo commandInfo,
            List<ArgumentInfo> parameterValues)
        {
            parameterValues = parameterValues ?? new List<ArgumentInfo>();

            try
            {
                //create instance
                object instance = AppInstanceCreator.CreateInstance(_type, _constrcutorParamValues);

                //dentify method to invove
                MethodInfo theMethod = instance.GetType().GetMethod(commandInfo.MethodName);

                //get values for method invokation
                object[] parameters = parameterValues.Select(ValueMachine.GetValue).ToArray();
                
                //invoke method
                object returnedObject = theMethod.Invoke(instance, parameters);

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