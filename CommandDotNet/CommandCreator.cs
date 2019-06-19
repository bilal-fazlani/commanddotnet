using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void CreateDefaultCommand(IApplicationMetadata applicationMetadata)
        {
            CommandInfo defaultCommandInfo = _type.GetDefaultCommandInfo(_settings);

            if (defaultCommandInfo != null)
            {
                ConfigureMetadata(_app, applicationMetadata);
                ConfigureCommandLineApplication(_app, defaultCommandInfo);
            }
            else
            {
                ConfigureMetadata(_app, applicationMetadata);
                _app.OnExecute(() =>
                {
                    _app.ShowHelp();
                    return Task.FromResult(0);
                });
            }
        }

        public void CreateCommands()
        {            
            foreach (CommandInfo commandInfo in _type.GetCommandInfos(_settings))
            {
                var command = _app.Command(commandInfo.Name, commandInfo.CustomAttributeProvider);
                ConfigureMetadata(command, commandInfo);
                ConfigureCommandLineApplication(command, commandInfo);
            }
        }

        private static void ConfigureMetadata(CommandLineApplication app, IApplicationMetadata applicationMetadata)
        {
            app.Description = applicationMetadata?.Description;
            app.ExtendedHelpText = applicationMetadata?.ExtendedHelpText;
            app.HelpOption(Constants.HelpTemplate);
        }

        private void ConfigureCommandLineApplication(CommandLineApplication app, CommandInfo commandInfo)
        {
            List<ArgumentInfo> argumentValues = new List<ArgumentInfo>();

            foreach (ArgumentInfo argument in commandInfo.Arguments)
            {
                argumentValues.Add(argument);
                switch (argument)
                {
                    case OptionArgumentInfo option:
                        SetValueForOption(option, app);
                        break;
                    case OperandArgumentInfo operand:
                        SetValueForParameter(operand, app);
                        break;
                }
            }

            app.OnExecute(async () => await _commandRunner.RunCommand(commandInfo, argumentValues));
        }

        private static void SetValueForParameter(OperandArgumentInfo operandArgument, CommandLineApplication command)
        {
            operandArgument.SetValue(command.Operand(
                operandArgument.Name,
                operandArgument.AnnotatedDescription,
                _=>{},
                operandArgument.TypeDisplayName,
                operandArgument.DefaultValue,
                operandArgument.IsMultipleType,
                operandArgument.AllowedValues));
        }

        private static void SetValueForOption(OptionArgumentInfo option, CommandLineApplication command)
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
                var optionInfo = (OptionArgumentInfo) argumentInfo;
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