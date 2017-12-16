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

        public void CreateDefaultSubCommand(CommandLineApplication command, List<ArgumentInfo> optionValues)
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

        public void CreateSubCommands(CommandLineApplication app, List<ArgumentInfo> optionValues)
        {            
            foreach (CommandInfo commandInfo in _type.GetCommandInfos(_settings))
            {
                List<ArgumentInfo> parameterValues = new List<ArgumentInfo>();

                CommandLineApplication commandOption = app.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;

                    command.ExtendedHelpText = commandInfo.ExtendedHelpText;

                    command.AllowArgumentSeparator = true;
                    
                    command.HelpOption(Constants.HelpTemplate);
                      
                    foreach (ArgumentInfo parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter);
                    }

                    foreach (var parameter in parameterValues)
                    {
                        parameter.SetValue(command.Option(parameter.Template,
                            parameter.EffectiveDescription,
                            parameter.CommandOptionType));
                    }
                });

                commandOption.OnExecute(async () => await _type.InvokeMethod(commandOption, commandInfo, parameterValues, optionValues));
            }
        }
    }
}