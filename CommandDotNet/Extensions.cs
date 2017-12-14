using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        
        public static Dictionary<ArgumentInfo, CommandOption> GetOptionValues(this Type type, CommandLineApplication command, AppSettings settings)
        {
            IEnumerable<ArgumentInfo> options = type
                .GetConstructors()
                .FirstOrDefault()
                .GetParameters()
                .Select(p => new ArgumentInfo(p, settings));

            Dictionary<ArgumentInfo, CommandOption> optionValues = new Dictionary<ArgumentInfo, CommandOption>();

            foreach (ArgumentInfo optionInfo in options)
            {
                optionValues.Add(optionInfo,
                    command.Option(optionInfo.Template, optionInfo.EffectiveDescription, optionInfo.CommandOptionType));
            }
            return optionValues;
        }

        public static Dictionary<ArgumentInfo, CommandOption> CreateDefaultCommand( 
            this Type type,
            CommandLineApplication command,
            AppSettings settings,
            Dictionary<ArgumentInfo, CommandOption> optionValues)
        {
            CommandInfo defaultCommandInfo = type.GetDefaultCommandInfo(settings);
            
            Dictionary<ArgumentInfo, CommandOption> defaultCommandParameterValues =
                new Dictionary<ArgumentInfo, CommandOption>();

            command.OnExecute(async () =>
            {
                if (defaultCommandInfo != null)
                {
                    if (defaultCommandInfo.Parameters.Any())
                    {
                        throw new Exception("Method with [DefaultMethod] attribute does not support parameters");
                    }

                    return await type.InvokeMethod(command, defaultCommandInfo, defaultCommandParameterValues, optionValues);
                }

                command.ShowHelp();
                return 0;
            });

            return defaultCommandParameterValues;
        }

        public static void CreateCommands(this Type type, CommandLineApplication command, 
            AppSettings settings, Dictionary<ArgumentInfo, CommandOption> optionValues)
        {            
            foreach (CommandInfo commandInfo in type.GetCommandInfos(settings))
            {
                Dictionary<ArgumentInfo, CommandOption> parameterValues =
                    new Dictionary<ArgumentInfo, CommandOption>();

                CommandLineApplication subCommandOption = command.Command(commandInfo.Name, subCommand =>
                {
                    subCommand.Description = commandInfo.Description;

                    subCommand.ExtendedHelpText = commandInfo.ExtendedHelpText;

                    subCommand.HelpOption(Constants.HelpTemplate);
                        
                    foreach (ArgumentInfo parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter, subCommand.Option(parameter.Template,
                            parameter.EffectiveDescription,
                            parameter.CommandOptionType));
                    }
                });

                subCommandOption.OnExecute(async () => await type.InvokeMethod(subCommandOption, commandInfo, parameterValues, optionValues));
            }
        }

        public static void AddDetails(this Type type, CommandLineApplication command, string name = null)
        {
            ApplicationMetadataAttribute consoleApplicationAttribute =
                type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            command.Name = name ?? consoleApplicationAttribute?.Name;
            
            command.HelpOption(Constants.HelpTemplate);

            command.FullName = consoleApplicationAttribute?.Description;

            command.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;
        }

        public static CommandLineApplication CreateApp(this Type type, AppSettings settings, string name = null)
        {
            CommandLineApplication command = new CommandLineApplication();
            
            Dictionary<ArgumentInfo, CommandOption> optionValues = type.GetOptionValues(command, settings);
            
            type.AddDetails(command, name);
                
            type.CreateDefaultCommand(command, settings, optionValues);
                
            type.CreateCommands(command, settings, optionValues);

            return command;
        }
        
        private static async Task<int> InvokeMethod(
            this Type type,
            CommandLineApplication command,
            CommandInfo commandInfo, 
            Dictionary<ArgumentInfo, CommandOption> parameterValues,
            Dictionary<ArgumentInfo, CommandOption> optionValues)
        {
            try
            {
                object instance = AppFactory.CreateApp(type, optionValues);
                
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