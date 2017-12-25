using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class CommandCreator
    {
        private readonly Type _type;
        private readonly CommandLineApplication _app;
        private readonly AppSettings _settings;
        private readonly CommandRunner _commandRunner;

        public CommandCreator(Type type, CommandLineApplication app, AppSettings settings)
        {
            _type = type;
            _app = app;
            _settings = settings;
            
            //get values for construtor params
            List<ArgumentInfo> constructorValues = GetOptionValuesForConstructor();
            
            _commandRunner = new CommandRunner(app, type, constructorValues, settings);
        }

        public void CreateDefaultCommand()
        {
            CommandInfo defaultCommandInfo = _type.GetDefaultCommandInfo(_settings);
            
            _app.OnExecute(async () =>
            {
                if (defaultCommandInfo != null)
                {
                    if (defaultCommandInfo.Parameters.Any())
                    {
                        throw new Exception("Method with [DefaultMethod] attribute does not support parameters");
                    }

                    return await _commandRunner.RunCommand(defaultCommandInfo, null);
                }

                _app.ShowHelp();
                return 0;
            });
        }

        public void CreateCommands()
        {            
            foreach (CommandInfo commandInfo in _type.GetCommandInfos(_settings))
            {
                List<ArgumentInfo> parameterValues = new List<ArgumentInfo>();

                CommandLineApplication commandOption = _app.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;

                    command.ExtendedHelpText = commandInfo.ExtendedHelpText;

                    command.AllowArgumentSeparator = _settings.AllowArgumentSeparator;

                    command.Syntax = commandInfo.Syntax;
                    
                    command.HelpOption(Constants.HelpTemplate);
                      
                    foreach (ArgumentInfo parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter);
                    }

                    foreach (var parameter in parameterValues)
                    {
                        parameter.SetValue(command.Option(parameter.Template,
                            parameter.EffectiveDescription,
                            parameter.CommandOptionType, option =>
                            {
                                option.ShowInHelpText = !parameter.IsSubject;
                            }), parameter.IsSubject ? command.RemainingArguments : null);
                    }
                }, throwOnUnexpectedArg: _settings.ThrowOnUnexpectedArgument);

                commandOption.OnExecute(async () => await _commandRunner.RunCommand(commandInfo, parameterValues));
            }
        }
        
        private List<ArgumentInfo> GetOptionValuesForConstructor()
        {
            List<ArgumentInfo> arguments = _type
                .GetConstructors()
                .FirstOrDefault()
                .GetParameters()
                .Select(p => new ArgumentInfo(p, _settings))
                .ToList();
            
            foreach (ArgumentInfo argumentInfo in arguments)
            {
                argumentInfo.SetValue(_app.Option(
                    argumentInfo.Template, 
                    argumentInfo.EffectiveDescription, 
                    argumentInfo.CommandOptionType, option =>
                    {
                        option.ShowInHelpText = !argumentInfo.IsSubject;
                    }), argumentInfo.IsSubject ? _app.RemainingArguments : null);
            }
            
            return arguments;
        }
    }
}