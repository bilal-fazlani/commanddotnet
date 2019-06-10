using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class CommandCreator
    {
        private readonly Type _type;
        private readonly CommandLineApplication _app;
        private readonly AppSettings _settings;
        private readonly CommandRunner _commandRunner;

        public CommandCreator(Type type, CommandLineApplication app, IDependencyResolver dependencyResolver, AppSettings settings)
        {
            _type = type;
            _app = app;
            _settings = settings;
            
            //get values for constructor params
            IEnumerable<ArgumentInfo> constructorValues = GetOptionValuesForConstructor();
            
            _commandRunner = new CommandRunner(type, constructorValues, dependencyResolver, settings);
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
                        throw new AppRunnerException("Method with [DefaultMethod] attribute does not support parameters");
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

                    command.Syntax = commandInfo.Syntax;
                    
                    command.HelpOption(Constants.HelpTemplate);
                      
                    foreach (ArgumentInfo argument in commandInfo.Arguments)
                    {
                        argumentValues.Add(argument);
                        switch (argument)
                        {
                            case CommandOptionInfo option:
                                SetValueForOption(option, command);
                                break;
                            case CommandParameterInfo parameter:
                                SetValueForParameter(parameter, command);
                                break;
                        }
                    }
                });

                commandOption.OnExecute(async () => await _commandRunner.RunCommand(commandInfo, argumentValues));
            }
        }

        private static void SetValueForParameter(CommandParameterInfo parameter, CommandLineApplication command)
        {
            parameter.SetValue(command.Argument(
                parameter.Name,
                parameter.AnnotatedDescription,
                _=>{},
                parameter.TypeDisplayName,
                parameter.DefaultValue,
                parameter.IsMultipleType,
                parameter.AllowedValues));
        }

        private static void SetValueForOption(CommandOptionInfo option, CommandLineApplication command)
        {
            option.SetValue(command.Option(option.Template,
                option.AnnotatedDescription,
                option.CommandOptionType,
                _=>{},
                option.Inherited,
                option.TypeDisplayName,
                option.DefaultValue,
                option.IsMultipleType,
                option.AllowedValues));
        }

        private IEnumerable<ArgumentInfo> GetOptionValuesForConstructor()
        {
            var firstCtor = _type
                .GetConstructors()
                .FirstOrDefault();
            
            List<ArgumentInfo> argumentInfos = new ArgumentInfoCreator(_settings)
                .GetArgumentsFromMethod(firstCtor)
                .ToList();

            foreach (ArgumentInfo argumentInfo in argumentInfos)
            {
                var optionInfo = (CommandOptionInfo) argumentInfo;
                optionInfo.SetValue(_app.Option(
                    optionInfo.Template,
                    optionInfo.AnnotatedDescription,
                    optionInfo.CommandOptionType,
                    _=>{},
                    optionInfo.Inherited,
                    optionInfo.TypeDisplayName,
                    optionInfo.DefaultValue,
                    optionInfo.IsMultipleType,
                    optionInfo.AllowedValues));
            }
            
            return argumentInfos;
        }
    }
}