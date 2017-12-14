using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class CommandCreator
    {
        private readonly Type _type;
        private readonly AppSettings _settings;

        public CommandCreator(Type type, AppSettings settings)
        {
            _type = type;
            _settings = settings;
        }

        public CommandLineApplication CreateCommand(string name)
        {
            CommandLineApplication command = new CommandLineApplication();
            
            List<ArgumentInfo> optionValues = _type.GetOptionValues(command, _settings);
                            
            ApplicationMetadataAttribute consoleApplicationAttribute =  _type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            command.Name = name ?? consoleApplicationAttribute?.Name;
            
            command.HelpOption(Constants.HelpTemplate);

            command.FullName = consoleApplicationAttribute?.Description;

            command.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;
            
            CreateDefaultSubCommand(command, optionValues);
                
            CreateSubCommands(command, optionValues);

            return command;
        }
        
        private void CreateDefaultSubCommand(CommandLineApplication command, List<ArgumentInfo> optionValues)
        {
            CommandInfo defaultCommandInfo = _type.GetDefaultCommandInfo(_settings);
            
            command.OnExecute(async () =>
            {
                if (defaultCommandInfo != null)
                {
                    if (defaultCommandInfo.Parameters.Any())
                    {
                        throw new Exception("Method with [DefaultMethod] attribute does not support parameters");
                    }

                    return await _type.InvokeMethod(command, defaultCommandInfo, null, optionValues);
                }

                command.ShowHelp();
                return 0;
            });
        }

        private void CreateSubCommands(CommandLineApplication command, List<ArgumentInfo> optionValues)
        {            
            foreach (CommandInfo commandInfo in _type.GetCommandInfos(_settings))
            {
                List<ArgumentInfo> parameterValues = new List<ArgumentInfo>();

                CommandLineApplication subCommandOption = command.Command(commandInfo.Name, subCommand =>
                {
                    subCommand.Description = commandInfo.Description;

                    subCommand.ExtendedHelpText = commandInfo.ExtendedHelpText;

                    subCommand.HelpOption(Constants.HelpTemplate);
                      
                    foreach (ArgumentInfo parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter);
                    }

                    foreach (var parameter in parameterValues)
                    {
                        parameter.SetValue(subCommand.Option(parameter.Template,
                            parameter.EffectiveDescription,
                            parameter.CommandOptionType));
                    }
                });

                subCommandOption.OnExecute(async () => await _type.InvokeMethod(subCommandOption, commandInfo, 
                    parameterValues, optionValues));
            }
        }
    }
}