using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using CommandDotNet.Invocation;

namespace CommandDotNet.ClassModeling
{
    internal class CommandCreator
    {
        private readonly Type _type;
        private readonly Command _command;
        private readonly AppSettings _settings;
        private readonly IOptionSource[] _optionSources;
        private readonly CommandRunner _commandRunner;

        public CommandCreator(Type type, Command command, IDependencyResolver dependencyResolver,
            AppSettings settings, IOptionSource[] optionSources)
        {
            _type = type;
            _command = command;
            _settings = settings;
            _optionSources = optionSources;

            //get values for constructor params
            IEnumerable<ArgumentInfo> constructorValues = GetOptionValuesForConstructor();
            
            _commandRunner = new CommandRunner(type, constructorValues, dependencyResolver, settings);
        }

        public void CreateDefaultCommand(IApplicationMetadata applicationMetadata)
        {
            CommandInfo defaultCommandInfo = _type.GetDefaultCommandInfo(_settings);

            if (defaultCommandInfo != null)
            {
                ConfigureMetadata(_command, applicationMetadata);
                ConfigureCommand(_command, defaultCommandInfo);
            }
            else
            {
                ConfigureMetadata(_command, applicationMetadata);
                _command.OnExecute(() =>
                {
                    HelpOptionSource.Print(_settings, _command);
                    return Task.FromResult(0);
                });
            }
        }

        public void CreateCommands()
        {            
            foreach (CommandInfo commandInfo in _type.GetCommandInfos(_settings))
            {
                var command = _command.AddCommand(commandInfo.Name, commandInfo.CustomAttributeProvider);
                ConfigureMetadata(command, commandInfo);
                ConfigureCommand(command, commandInfo);
            }
        }

        private void ConfigureMetadata(Command command, IApplicationMetadata applicationMetadata)
        {
            command.Description = applicationMetadata?.Description;
            command.ExtendedHelpText = applicationMetadata?.ExtendedHelpText;
            _optionSources.ForEach(s => s.AddOption(command));
        }

        private void ConfigureCommand(Command command, CommandInfo commandInfo)
        {
            List<ArgumentInfo> argumentValues = new List<ArgumentInfo>();

            foreach (ArgumentInfo argument in commandInfo.Arguments)
            {
                argumentValues.Add(argument);
                switch (argument)
                {
                    case OptionArgumentInfo option:
                        SetValueForOption(option, command);
                        break;
                    case OperandArgumentInfo operand:
                        SetValueForOperand(operand, command);
                        break;
                }
            }

            command.OnExecute(async () => await _commandRunner.RunCommand(commandInfo, argumentValues));
        }

        private static void SetValueForOperand(OperandArgumentInfo operandInfo, Command command)
        {
            var operand = command.AddOperand(
                operandInfo.Name,
                operandInfo.Description,
                operandInfo.Arity,
                operandInfo.TypeDisplayName,
                operandInfo.DefaultValue,
                operandInfo.AllowedValues);
            operand.ContextData.Set<ArgumentInfo>(operandInfo);
            operandInfo.SetValueInfo(operand, operand.SetValues);
        }

        private static void SetValueForOption(OptionArgumentInfo optionInfo, Command command)
        {
            var option = command.AddOption(optionInfo.Template,
                optionInfo.Description,
                optionInfo.Arity,
                optionInfo.Inherited,
                optionInfo.TypeDisplayName,
                optionInfo.DefaultValue,
                optionInfo.AllowedValues);
            option.ContextData.Set<ArgumentInfo>(optionInfo);
            optionInfo.SetValueInfo(option, option.SetValues);
        }

        private IEnumerable<ArgumentInfo> GetOptionValuesForConstructor()
        {
            var firstCtor = _type
                .GetConstructors()
                .FirstOrDefault();
            
            List<ArgumentInfo> argumentInfos = new ArgumentInfoCreator(_settings)
                .GetArgumentsFromMethod(firstCtor)
                .ToList();
            
            argumentInfos
                .Cast<OptionArgumentInfo>()
                .ForEach(o => SetValueForOption(o, _command));
            
            return argumentInfos;
        }
    }
}