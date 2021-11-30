using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class DefinitionMappingExtensions
    {
        internal static ICommandBuilder ToCommand(this ICommandDef commandDef, Command? parent, CommandContext commandContext)
        {
            var command = new Command(
                commandDef.Name, 
                commandDef.CustomAttributes,
                commandDef.IsExecutable,
                parent,
                commandDef.SourcePath);
            command.Services.AddOrUpdate(commandDef);

            var commandAttribute = commandDef.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute != null)
            {
                command.Description = JoinFromAttribute(commandDef, nameof(commandAttribute.Description), commandAttribute.Description, commandAttribute.DescriptionLines);
                command.Usage = JoinFromAttribute(commandDef, nameof(commandAttribute.Usage), commandAttribute.Usage, commandAttribute.UsageLines);
                command.ExtendedHelpText = JoinFromAttribute(commandDef, nameof(commandAttribute.ExtendedHelpText), commandAttribute.ExtendedHelpText, commandAttribute.ExtendedHelpTextLines);
            }

            var appSettings = commandContext.AppConfig.AppSettings;
            command.IgnoreUnexpectedOperands = commandAttribute?.IgnoreUnexpectedOperandsAsNullable ?? appSettings.Parser.IgnoreUnexpectedOperands;
            command.ArgumentSeparatorStrategy = commandAttribute?.ArgumentSeparatorStrategyAsNullable ?? appSettings.Parser.DefaultArgumentSeparatorStrategy;

            var commandBuilder = new CommandBuilder(command);

            commandDef.InvokeMethodDef?.Arguments
                .ForEach(commandBuilder.AddArgument);

            commandDef.InterceptorMethodDef?.Arguments
                .ForEach(commandBuilder.AddArgument);

            commandDef.SubCommands
                .Select(c => c.ToCommand(command, commandContext).Command)
                .ForEach(commandBuilder.AddSubCommand);

            commandContext.AppConfig.BuildEvents.CommandCreated(commandContext, commandBuilder);

            return commandBuilder;
        }

        internal static IArgument ToArgument(this IArgumentDef argumentDef, AppConfig appConfig, bool isInterceptorOption)
        {
            var underlyingType = argumentDef.Type.GetUnderlyingType();

            var argument = BuildArgument(
                argumentDef,
                new TypeInfo(argumentDef.Type, underlyingType), 
                isInterceptorOption);
            
            argument.Services.AddOrUpdate(argumentDef);
            if (argumentDef.CustomAttributes is ParameterInfo param)
            {
                argument.Services.Add(param);
            }
            else if (argumentDef.CustomAttributes is PropertyInfo prop)
            {
                argument.Services.Add(prop);
            }

            var typeDescriptor = appConfig.AppSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(underlyingType);
            argument.TypeInfo.DisplayName = typeDescriptor.GetDisplayName(argument);

            if (typeDescriptor is IAllowedValuesTypeDescriptor allowedValuesTypeDescriptor)
            {
                argument.AllowedValues = allowedValuesTypeDescriptor.GetAllowedValues(argument).ToReadOnlyCollection();
            }
            return argument;
        }

        private static IArgument BuildArgument(IArgumentDef argumentDef, TypeInfo typeInfo, bool isInterceptorOption)
        {
            var argumentDefault = argumentDef.HasDefaultValue && !argumentDef.DefaultValue.IsNullValue()
                ? new ArgumentDefault($"app.{argumentDef.ArgumentDefType}", argumentDef.SourcePath, argumentDef.DefaultValue!)
                : null;

            if (argumentDef.CommandNodeType == CommandNodeType.Operand)
            {
                var operandAttr = argumentDef.GetCustomAttribute<OperandAttribute>();
                return new Operand(
                    argumentDef.Name,
                    typeInfo,
                    argumentDef.Arity,
                    argumentDef.BooleanMode,
                    argumentDef.SourcePath,
                    customAttributes: argumentDef.CustomAttributes,
                    argumentDef.ValueProxy)
                {
                    Description = JoinFromAttribute(argumentDef, nameof(operandAttr.Description), operandAttr?.Description, operandAttr?.DescriptionLines),
                    Default = argumentDefault
                };
            }
            
            if (argumentDef.CommandNodeType == CommandNodeType.Option)
            {
                var optionAttr = argumentDef.GetCustomAttribute<OptionAttribute>();
                var assignOnlyToExecutableSubcommands = optionAttr?.AssignToExecutableSubcommands ?? false;
                isInterceptorOption = isInterceptorOption && !assignOnlyToExecutableSubcommands;
                
                return new Option(
                    ParseLongName(argumentDef, optionAttr),
                    optionAttr?.ShortName,
                    typeInfo,
                    argumentDef.Arity,
                    argumentDef.BooleanMode,
                    definitionSource: argumentDef.SourcePath,
                    customAttributes: argumentDef.CustomAttributes,
                    isInterceptorOption: isInterceptorOption,
                    assignToExecutableSubcommands: assignOnlyToExecutableSubcommands,
                    valueProxy: argumentDef.ValueProxy)
                {
                    Description = JoinFromAttribute(argumentDef, nameof(optionAttr.Description), optionAttr?.Description, optionAttr?.DescriptionLines),
                    Split = argumentDef.Split,
                    Default = argumentDefault
                };
            }

            throw new ArgumentOutOfRangeException($"Unknown argument type: {argumentDef.CommandNodeType}");
        }

        private static string? JoinFromAttribute(ISourceDef sourceDef, string propertyName, string? singleline, string[]? multiline)
        {
            if (singleline is not null && multiline is not null)
            {
                throw new InvalidConfigurationException(
                    $"Both {propertyName} and {propertyName}Lines were set for {sourceDef.SourcePath}. Only one can be set.");
            }

            return singleline ?? multiline?.ToCsv(Environment.NewLine);
        }

        private static string? ParseLongName(IArgumentDef argumentDef, OptionAttribute? optionAttr)
        {
            if (optionAttr == null)
            {
                return argumentDef.Name;
            }

            if (optionAttr.LongName != null)
            {
                return optionAttr.LongName;
            }

            return optionAttr.NoLongName ? null : argumentDef.Name;
        }

        internal static char? GetSplitChar(this IArgumentDef argumentDef)
        {
            OptionAttribute? optionAttr = argumentDef.GetCustomAttribute<OptionAttribute>();
            return optionAttr?.SplitAsNullable is null
                ? null
                : argumentDef.Type.IsNonStringEnumerable()
                    ? optionAttr.SplitAsNullable
                    : throw new InvalidConfigurationException(
                        $"Split can only be specified for IEnumerable<T> types. {argumentDef.SourcePath} is type {argumentDef.Type}");
        }

        internal static BooleanMode? GetBooleanMode(
            this IArgumentDef argumentDef, BooleanMode defaultBooleanMode)
        {
            if (argumentDef.Type != typeof(bool) && argumentDef.Type != typeof(bool?))
            {
                return null;
            }

            if (argumentDef.CommandNodeType == CommandNodeType.Operand)
            {
                return BooleanMode.Explicit;
            }

            OptionAttribute? optionAttr = argumentDef.GetCustomAttribute<OptionAttribute>();
            
            if (optionAttr?.BooleanModeAsNullable == null)
                return defaultBooleanMode;

            return argumentDef.Type.GetUnderlyingType() == typeof(bool)
                ? optionAttr.BooleanMode
                : throw new InvalidConfigurationException(
                    $"BooleanMode is set to `{optionAttr.BooleanMode}` for a non boolean type. {argumentDef}");
        }
    }
}