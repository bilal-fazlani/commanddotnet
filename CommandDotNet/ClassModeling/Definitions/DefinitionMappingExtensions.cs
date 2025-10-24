using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.ClassModeling.Definitions;

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
            var operand = new Operand(
                argumentDef.Name,
                typeInfo,
                argumentDef.Arity,
                argumentDef.BooleanMode,
                argumentDef.SourcePath,
                customAttributes: argumentDef.CustomAttributes,
                argumentDef.ValueProxy)
            {
                Default = argumentDefault
            };
            
            SetDescription(operand, argumentDef, nameof(operandAttr.Description), operandAttr?.Description, operandAttr?.DescriptionLines);
            return operand;
        }
            
        if (argumentDef.CommandNodeType == CommandNodeType.Option)
        {
            var optionAttr = argumentDef.GetCustomAttribute<OptionAttribute>();
            var assignOnlyToExecutableSubcommands = optionAttr?.AssignToExecutableSubcommands ?? false;
            isInterceptorOption = isInterceptorOption && !assignOnlyToExecutableSubcommands;
                
            var option = new Option(
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
                Split = argumentDef.Split,
                Default = argumentDefault
            };
            
            SetDescription(option, argumentDef, nameof(optionAttr.Description), optionAttr?.Description, optionAttr?.DescriptionLines);
            return option;
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

    /// <summary>
    /// Sets the description on an argument from attribute sources.
    /// Ensures only one is set: a single-line description, a multi-line description, or a <see cref="DescriptionMethodAttribute"/>.
    /// If a <see cref="DescriptionMethodAttribute"/> is used, resolves the method early and validates it.
    /// </summary>
    private static void SetDescription(
        IArgument argument,
        IArgumentDef argumentDef,
        string propertyName,
        string? singleline,
        string[]? multiline)
    {
        var descriptionMethodAttr = argumentDef.GetCustomAttribute<DescriptionMethodAttribute>();

        var setProperties = new List<string>();
        if (singleline is not null) setProperties.Add(propertyName);
        if (multiline is not null) setProperties.Add($"{propertyName}Lines");
        if (descriptionMethodAttr is not null) setProperties.Add(nameof(DescriptionMethodAttribute));

        if (setProperties.Count > 1)
        {
            throw new InvalidConfigurationException(
                $"Multiple description properties were set for {argumentDef.SourcePath}: {string.Join(", ", setProperties)}. Only one can be set.");
        }

        if (descriptionMethodAttr is not null)
        {
            // Resolve the method early to catch configuration errors
            var methodName = descriptionMethodAttr.MethodName;
            
            // Get the declaring type from the argument's custom attributes
            Type? declaringType = null;
            
            if (argumentDef.CustomAttributes is ParameterInfo paramInfo)
            {
                declaringType = paramInfo.Member.DeclaringType;
            }
            else if (argumentDef.CustomAttributes is PropertyInfo propInfo)
            {
                declaringType = propInfo.DeclaringType;
            }
            
            if (declaringType == null)
            {
                throw new InvalidConfigurationException(
                    $"Could not determine declaring type for DescriptionMethod '{methodName}' on {argumentDef.SourcePath}");
            }
            
            // Find the method
            var method = declaringType.GetMethod(methodName, 
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);
                
            if (method == null)
            {
                throw new InvalidConfigurationException(
                    $"DescriptionMethod '{methodName}' not found in type '{declaringType.Name}'. " +
                    $"Method must be static with no parameters and return string. " +
                    $"Defined on {argumentDef.SourcePath}");
            }
            
            if (method.ReturnType != typeof(string))
            {
                throw new InvalidConfigurationException(
                    $"DescriptionMethod '{methodName}' in type '{declaringType.Name}' must return string but returns {method.ReturnType.Name}. " +
                    $"Defined on {argumentDef.SourcePath}");
            }
            
            // Set the method as a lambda that invokes it
            if (argument is Option option)
            {
                option.DescriptionMethod = () => (string?)method.Invoke(null, null);
            }
            else if (argument is Operand operand)
            {
                operand.DescriptionMethod = () => (string?)method.Invoke(null, null);
            }
        }
        else
        {
            // Set static description
            var description = singleline ?? multiline?.ToCsv(Environment.NewLine);
            if (argument is Option option)
            {
                option.Description = description;
            }
            else if (argument is Operand operand)
            {
                operand.Description = description;
            }
        }
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