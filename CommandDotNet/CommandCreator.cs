using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
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
                    if (defaultCommandInfo.Arguments.Any())
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
                List<ArgumentInfo> argumentValues = new List<ArgumentInfo>();

                CommandLineApplication commandOption = _app.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;

                    command.ExtendedHelpText = commandInfo.ExtendedHelpText;

                    command.AllowArgumentSeparator = _settings.AllowArgumentSeparator;

                    command.Syntax = commandInfo.Syntax;
                    
                    command.HelpOption(Constants.HelpTemplate);
                      
                    foreach (ArgumentInfo argument in commandInfo.Arguments)
                    {
                        argumentValues.Add(argument);
                    }

                    foreach (var option in argumentValues.OfType<CommandOptionInfo>())
                    {
                        option.SetValue(command.Option(option.Template,
                            option.EffectiveDescription,
                            option.CommandOptionType));
                    }
                    
                    foreach (var parameter in argumentValues.OfType<CommandParameterInfo>())
                    {
                        parameter.SetValue(command.Argument(
                            parameter.Name, 
                            parameter.EffectiveDescription, 
                            parameter.IsMultipleType));
                    }
                    
                }, throwOnUnexpectedArg: _settings.ThrowOnUnexpectedArgument);

                commandOption.OnExecute(async () => await _commandRunner.RunCommand(commandInfo, argumentValues));
            }
        }
        
        private List<ArgumentInfo> GetOptionValuesForConstructor()
        {
            IEnumerable<ParameterInfo> parameterInfos = _type
                .GetConstructors()
                .FirstOrDefault()
                .GetParameters();

            if(parameterInfos.Any(p => p.HasAttribute<ParameterAttribute>()))
                throw new AppRunnerException("Constructor arguments can not have [Parameter] attribute. Please use [Option] attribute");
            
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();
            
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.HasAttribute<ParameterAttribute>())
                {
                    arguments.Add(new CommandParameterInfo(parameterInfo, _settings));
                }
                else
                {
                    arguments.Add(new CommandOptionInfo(parameterInfo, _settings));
                }
            }

            foreach (var argumentInfo in arguments)
            {
                switch (argumentInfo)
                {
                    case CommandParameterInfo parameterInfo:
                        parameterInfo.SetValue(_app.Argument(parameterInfo.Name,
                            parameterInfo.EffectiveDescription, parameterInfo.IsMultipleType));
                        break;
                    case CommandOptionInfo optionInfo:
                        optionInfo.SetValue(_app.Option(
                            optionInfo.Template,
                            optionInfo.EffectiveDescription,
                            optionInfo.CommandOptionType));
                        break;
                }
            }

            return arguments;
        }
    }
}