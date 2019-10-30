using System;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class DefinitionMappingExtensions
    {
        internal static ICommandBuilder ToCommand(this ICommandDef commandDef, Command parent, CommandContext commandContext)
        {
            var command = new Command(
                commandDef.Name, 
                commandDef.CustomAttributes,
                commandDef.IsExecutable,
                parent,
                commandDef.SourcePath);
            command.Services.AddOrUpdate(commandDef);

            var commandAttribute = commandDef.CustomAttributes.GetCustomAttribute<CommandAttribute>() 
                                   ?? commandDef.CustomAttributes.GetCustomAttribute<ApplicationMetadataAttribute>();
            if (commandAttribute != null)
            {
                command.Description = commandAttribute.Description;
                command.Usage = commandAttribute.Usage;
                command.ExtendedHelpText = commandAttribute.ExtendedHelpText;
            }

            var commandBuilder = new CommandBuilder(command);

            commandDef.InvokeMethodDef.ArgumentDefs
                .Select(a => a.ToArgument(command, commandContext.AppConfig, false))
                .ForEach(commandBuilder.AddArgument);

            commandDef.InterceptorMethodDef.ArgumentDefs
                .Select(a => a.ToArgument(command, commandContext.AppConfig, true))
                .ForEach(commandBuilder.AddArgument);

            if (commandDef.IsExecutable)
            {
                command.GetParentCommands()
                    .SelectMany(c => c.Options.Where(o => o.Inherited))
                    .ForEach(commandBuilder.AddArgument);
            }

            commandDef.SubCommands
                .Select(c => c.ToCommand(command, commandContext).Command)
                .ForEach(commandBuilder.AddSubCommand);

            commandContext.AppConfig.BuildEvents.CommandCreated(commandContext, commandBuilder);

            return commandBuilder;
        }

        private static IArgument ToArgument(this IArgumentDef argumentDef, Command parent, AppConfig appConfig, bool isInterceptorOption)
        {
            var underlyingType = argumentDef.Type.GetUnderlyingType();

            var argument = BuildArgument(
                argumentDef, 
                parent,
                appConfig,
                new TypeInfo(argumentDef.Type, underlyingType), 
                isInterceptorOption);
            argumentDef.Argument = argument;
            argument.Services.AddOrUpdate(argumentDef);

            var typeDescriptor = appConfig.AppSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(underlyingType);
            argument.TypeInfo.DisplayName = typeDescriptor.GetDisplayName(argument);

            if (typeDescriptor is IAllowedValuesTypeDescriptor allowedValuesTypeDescriptor)
            {
                argument.AllowedValues = allowedValuesTypeDescriptor.GetAllowedValues(argument).ToReadOnlyCollection();
            }
            return argument;
        }

        private static IArgument BuildArgument(IArgumentDef argumentDef,
            Command parent,
            AppConfig appConfig,
            TypeInfo typeInfo,
            bool isInterceptorOption)
        {
            var defaultValue = argumentDef.HasDefaultValue ? argumentDef.DefaultValue : null;

            if (argumentDef.CommandNodeType == CommandNodeType.Operand)
            {
                var operandAttr = argumentDef.CustomAttributes.GetCustomAttribute<OperandAttribute>() 
                                  ?? (INameAndDescription) argumentDef.CustomAttributes.GetCustomAttribute<ArgumentAttribute>();
                return new Operand(
                    argumentDef.Name, 
                    parent, 
                    typeInfo,
                    ArgumentArity.Default(argumentDef.Type, argumentDef.HasDefaultValue, BooleanMode.Explicit), 
                    argumentDef.SourcePath,
                    customAttributes: argumentDef.CustomAttributes,
                    argumentDef.ValueProxy)
                {
                    Description = operandAttr?.Description,
                    DefaultValue = defaultValue
                };
            }
            
            if (argumentDef.CommandNodeType == CommandNodeType.Option)
            {
                var optionAttr = argumentDef.CustomAttributes.GetCustomAttribute<OptionAttribute>();
                var booleanMode = GetOptionBooleanMode(argumentDef, appConfig.AppSettings.BooleanMode, optionAttr);
                var argumentArity = ArgumentArity.Default(argumentDef.Type, argumentDef.HasDefaultValue, booleanMode);

                var longName = optionAttr?.ShortName != null 
                    ? optionAttr?.LongName 
                    : (optionAttr?.LongName ?? argumentDef.Name);
                return new Option(
                    longName,
                    ParseShortName(argumentDef, optionAttr?.ShortName),
                    parent, 
                    typeInfo, 
                    argumentArity, 
                    definitionSource: argumentDef.SourcePath,
                    customAttributes: argumentDef.CustomAttributes,
                    isInterceptorOption: isInterceptorOption,
                    valueProxy: argumentDef.ValueProxy)
                {
                    Description = optionAttr?.Description,
                    Inherited = optionAttr?.Inherited ?? false,
                    DefaultValue = defaultValue
                };
            }

            throw new ArgumentOutOfRangeException($"Unknown argument type: {argumentDef.CommandNodeType}");
        }

        private static char? ParseShortName(IArgumentDef argumentDef, string shortNameAsString)
        {
            if (shortNameAsString.IsNullOrWhitespace())
            {
                return null;
            }

            if (shortNameAsString.Length > 1)
            {
                throw new ArgumentException($"Short name must be a single character: {shortNameAsString} {argumentDef}",
                    nameof(shortNameAsString));
            }

            return shortNameAsString.Single();
        }

        private static BooleanMode GetOptionBooleanMode(
            IArgumentDef argumentDef, BooleanMode appBooleanMode, OptionAttribute optionAttr)
        {
            if (optionAttr == null || optionAttr.BooleanMode == BooleanMode.Unknown)
                return appBooleanMode;

            return argumentDef.Type.GetUnderlyingType() == typeof(bool)
                ? optionAttr.BooleanMode
                : throw new AppRunnerException(
                    $"BooleanMode is set to `{optionAttr.BooleanMode}` for a non boolean type. {argumentDef}");
        }
    }
}