using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Invocation;

namespace CommandDotNet.ClassModeling
{
    internal class CommandCreator
    {
        private readonly Type _type;
        private readonly Command _command;
        private readonly CommandContext _commandContext;
        private readonly AppSettings _settings;
        private readonly CommandRunner _commandRunner;

        public CommandCreator(Type type, Command command, CommandContext commandContext)
        {
            _type = type;
            _command = command;
            _commandContext = commandContext;
            _settings = commandContext.AppSettings;

            //get values for constructor params
            IEnumerable<ArgumentInfo> constructorValues = GetOptionValuesForConstructor();
            
            _commandRunner = new CommandRunner(type, constructorValues, commandContext.ExecutionConfig.DependencyResolver, _settings);
        }

        public void CreateDefaultCommand(IApplicationMetadata applicationMetadata)
        {
            CommandInfo defaultCommandInfo = GetDefaultCommandInfo(_type, _settings);

            if (defaultCommandInfo != null)
            {
                ConfigureMetadata(_command, applicationMetadata);
                ConfigureCommand(_command, defaultCommandInfo);
            }
            else
            {
                ConfigureMetadata(_command, applicationMetadata);
            }
        }

        public void CreateCommands()
        {            
            foreach (CommandInfo commandInfo in GetCommandInfos(_type, _settings))
            {
                var command = _command.AddCommand(commandInfo.Name, commandInfo.CustomAttributeProvider);
                ConfigureMetadata(command, commandInfo);
                ConfigureCommand(command, commandInfo);
            }
        }

        private static IEnumerable<CommandInfo> GetCommandInfos(Type type, AppSettings settings)
        {
            return type.GetDeclaredMethods()
                .Where(m => !m.HasAttribute<DefaultMethodAttribute>())
                .Select(mi => new CommandInfo(mi, settings));
        }

        private static CommandInfo GetDefaultCommandInfo(Type type, AppSettings settings)
        {
            return type.GetDeclaredMethods()
                .Where(m => m.HasAttribute<DefaultMethodAttribute>())
                .Select(mi => new CommandInfo(mi, settings))
                .FirstOrDefault();
        }

        private void ConfigureMetadata(Command command, IApplicationMetadata applicationMetadata)
        {
            command.Description = applicationMetadata?.Description;
            command.ExtendedHelpText = applicationMetadata?.ExtendedHelpText;
            _commandContext.ExecutionConfig.BuildEvents.CommandCreated(_commandContext, command);
        }

        private void ConfigureCommand(Command command, CommandInfo commandInfo)
        {
            List<ArgumentInfo> argumentInfos = new List<ArgumentInfo>();

            foreach (ArgumentInfo argument in commandInfo.Arguments)
            {
                argumentInfos.Add(argument);
                switch (argument)
                {
                    case OptionArgumentInfo option:
                        AddOption(option, command);
                        break;
                    case OperandArgumentInfo operand:
                        AddOperand(operand, command);
                        break;
                }
            }

            command.ContextData.Add(commandInfo);
            command.ContextData.Add(_commandRunner);
        }

        private static void AddOperand(OperandArgumentInfo operandInfo, Command command)
        {
            var operand = command.AddOperand(
                operandInfo.Type,
                operandInfo.Name,
                operandInfo.Description,
                operandInfo.Arity,
                operandInfo.TypeDisplayName,
                operandInfo.DefaultValue,
                operandInfo.AllowedValues);
            operand.ContextData.Set<ArgumentInfo>(operandInfo);
        }

        private static void AddOption(OptionArgumentInfo optionInfo, Command command)
        {
            var option = command.AddOption(
                optionInfo.Type, 
                optionInfo.Template,
                optionInfo.Description,
                optionInfo.Arity,
                optionInfo.Inherited,
                optionInfo.TypeDisplayName,
                optionInfo.DefaultValue,
                optionInfo.AllowedValues);
            option.ContextData.Set<ArgumentInfo>(optionInfo);
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
                .ForEach(o => AddOption(o, _command));
            
            return argumentInfos;
        }
    }
}