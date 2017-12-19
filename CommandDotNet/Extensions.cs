using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public static class Extensions
    {
        public static IEnumerable<CommandInfo> GetCommandInfos(this Type type, AppSettings settings)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.GetCustomAttribute<DefaultMethodAttribute>() == null)
                .Select(mi => new CommandInfo(mi, settings));
        }

        public static void CreateSubApplications(this Type type, 
            AppSettings settings, 
            CommandLineApplication parentApplication)
        {
            IEnumerable<Type> propertySubmodules = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => !p.PropertyType.IsValueType)
                .Select(p => p.PropertyType);

            IEnumerable<Type> inlineClassSubmodules = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);            
            
            foreach (Type submoduleType in propertySubmodules.Union(inlineClassSubmodules))
            {
                AppCreator appCreator = new AppCreator(settings);
                appCreator.CreateApplication(submoduleType, parentApplication);
            }
        }
        
        public static CommandInfo GetDefaultCommandInfo(this Type type, AppSettings settings)
        {
            CommandInfo defaultCommandInfo = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.GetCustomAttribute<DefaultMethodAttribute>() != null)
                .Select(mi => new CommandInfo(mi, settings))
                .FirstOrDefault();
            
            return defaultCommandInfo;
        }
        
        public static List<ArgumentInfo> GetOptionValues(this Type type, 
            CommandLineApplication command, 
            AppSettings settings)
        {
            List<ArgumentInfo> arguments = type
                .GetConstructors()
                .FirstOrDefault()
                .GetParameters()
                .Select(p => new ArgumentInfo(p, settings))
                .ToList();
            
            foreach (ArgumentInfo argumentInfo in arguments)
            {
                argumentInfo.SetValue(command.Option(
                    argumentInfo.Template, 
                    argumentInfo.EffectiveDescription, 
                    argumentInfo.CommandOptionType, option =>
                    {
                        option.ShowInHelpText = !argumentInfo.IsSubject;
                    }), argumentInfo.IsSubject ? command.RemainingArguments : null);
            }
            
            return arguments;
        }

        public static async Task<int> InvokeMethod(
            this Type type,
            CommandLineApplication command,
            CommandInfo commandInfo, 
            List<ArgumentInfo> parameterValues = null,
            List<ArgumentInfo> optionValues = null,
            ArgumentInfo subjectValue = null)
        {
            parameterValues = parameterValues ?? new List<ArgumentInfo>();

            optionValues = optionValues ?? new List<ArgumentInfo>();
            
            try
            {
                object instance = AppInstanceCreator.CreateInstance(type, optionValues);
                
                MethodInfo theMethod = type.GetMethod(commandInfo.MethodName);

                object returnedObject = theMethod.Invoke(instance,
                    parameterValues.Select(ValueMachine.GetValue).ToArray());

                int returnCode = 0;

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
                return returnCode;
            }
            catch (ValueParsingException e)
            {
                throw new CommandParsingException(command, e.Message);
            }
        }
    }
}