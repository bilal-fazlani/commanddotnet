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
                commandDef.CustomAttributeProvider,
                commandDef.IsExecutable,
                parent);
            command.Services.Set(commandDef);
            commandDef.Command = command;

            var commandAttribute = commandDef.CustomAttributeProvider.GetCustomAttribute<CommandAttribute>() 
                                   ?? commandDef.CustomAttributeProvider.GetCustomAttribute<ApplicationMetadataAttribute>();
            if (commandAttribute != null)
            {
                command.Description = commandAttribute.Description;
                command.ExtendedHelpText = commandAttribute.ExtendedHelpText;
            }

            var commandBuilder = new CommandBuilder(command);

            commandDef.InvokeMethodDef.ArgumentDefs
                .Select(a => a.ToArgument(commandContext.AppConfig, false))
                .ForEach(commandBuilder.AddArgument);

            commandDef.InterceptorMethodDef.ArgumentDefs
                .Select(a => a.ToArgument(commandContext.AppConfig, true))
                .ForEach(commandBuilder.AddArgument);

            if (commandDef.IsExecutable)
            {
                command.GetParentCommands()
                    .SelectMany(c => c.Options.Where(o => o.Inherited))
                    .ForEach(commandBuilder.AddArgument);
            }

            commandContext.AppConfig.BuildEvents.CommandCreated(commandContext, commandBuilder);

            commandDef.SubCommands
                .Select(c => c.ToCommand(command, commandContext).Command)
                .ForEach(commandBuilder.AddSubCommand);
            return commandBuilder;
        }

        private static IArgument ToArgument(this IArgumentDef argumentDef, AppConfig appConfig, bool isInterceptorOption)
        {
            var underlyingType = argumentDef.Type.GetUnderlyingType();

            var argument = BuildArgument(
                argumentDef, 
                appConfig, 
                argumentDef.HasDefaultValue ? argumentDef.DefaultValue : null,
                new TypeInfo(argumentDef.Type, underlyingType), 
                isInterceptorOption);
            argumentDef.Argument = argument;
            argument.Services.Set(argumentDef);

            var typeDescriptor = appConfig.AppSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(underlyingType);
            argument.TypeInfo.DisplayName = typeDescriptor.GetDisplayName(argument);

            if (typeDescriptor is IAllowedValuesTypeDescriptor allowedValuesTypeDescriptor)
            {
                argument.AllowedValues = allowedValuesTypeDescriptor.GetAllowedValues(argument).ToReadOnlyCollection();
            }
            return argument;
        }

        private static IArgument BuildArgument(
            IArgumentDef argumentDef, 
            AppConfig appConfig, 
            object defaultValue,
            TypeInfo typeInfo, 
            bool isInterceptorOption)
        {
            if (argumentDef.ArgumentType == ArgumentType.Operand)
            {
                var operandAttr = argumentDef.Attributes.GetCustomAttribute<OperandAttribute>() 
                                  ?? (INameAndDescription) argumentDef.Attributes.GetCustomAttribute<ArgumentAttribute>();
                return new Operand(operandAttr?.Name ?? argumentDef.Name, typeInfo, argumentDef.Attributes)
                {
                    Description = operandAttr?.Description,
                    Arity = ArgumentArity.Default(argumentDef.Type, BooleanMode.Explicit),
                    DefaultValue = defaultValue
                };
            }

            var optionAttr = argumentDef.Attributes.GetCustomAttribute<OptionAttribute>();
            var booleanMode = GetOptionBooleanMode(argumentDef, appConfig.AppSettings.BooleanMode, optionAttr);
            var argumentArity = ArgumentArity.Default(argumentDef.Type, booleanMode);

            return new Option(
                optionAttr?.LongName ?? argumentDef.Name,
                ParseShortName(optionAttr?.ShortName),
                typeInfo, argumentArity, customAttributeProvider: argumentDef.Attributes, isInterceptorOption: isInterceptorOption)
            {
                Description = optionAttr?.Description,
                Inherited = optionAttr?.Inherited ?? false,
                DefaultValue = defaultValue
            };
        }

        private static char? ParseShortName(string shortNameAsString)
        {
            if (shortNameAsString.IsNullOrWhitespace())
            {
                return null;
            }

            if (shortNameAsString.Length > 1)
            {
                throw new ArgumentException($"Short name must be a single character: {shortNameAsString}",
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
                    $"BooleanMode is set to `{optionAttr.BooleanMode}` for a non boolean type.  {argumentDef}");
        }
    }
}