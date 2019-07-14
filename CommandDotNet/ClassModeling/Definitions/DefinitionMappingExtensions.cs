using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class DefinitionMappingExtensions
    {
        internal static ICommandBuilder ToCommand(this ICommandDef commandDef, ICommand parent, CommandContext commandContext)
        {
            var command = new Command(commandDef.Name, commandDef.CustomAttributeProvider, parent);
            command.ContextData.Set(commandDef);
            commandDef.Command = command;

            var metadataAttribute = commandDef.CustomAttributeProvider.GetCustomAttribute<ApplicationMetadataAttribute>();
            if (metadataAttribute != null)
            {
                command.Description = metadataAttribute.Description;
                command.ExtendedHelpText = metadataAttribute.ExtendedHelpText;
            }

            commandDef.Arguments
                .Select(a => a.ToArgument(commandContext.ExecutionConfig))
                .ForEach(a => command.AddArgument(a));

            commandContext.ExecutionConfig.BuildEvents.CommandCreated(commandContext, command);

            commandDef.SubCommands
                .Select(c => c.ToCommand(command, commandContext))
                .ForEach(c => command.AddSubCommand(c.Command));
            return command;
        }

        private static IArgument ToArgument(this IArgumentDef argumentDef, ExecutionConfig executionConfig)
        {
            var underlyingType = argumentDef.Type.GetUnderlyingType();

            var argument = BuildArgument(
                argumentDef, 
                executionConfig, 
                argumentDef.HasDefaultValue ? argumentDef.DefaultValue : null,
                new TypeInfo
                {
                    Type = argumentDef.Type,
                    UnderlyingType = underlyingType,
                }
            );
            argument.ContextData.Set(argumentDef);
            argumentDef.Argument = argument;

            var typeDescriptor = executionConfig.AppSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(underlyingType);
            argument.TypeInfo.DisplayName = typeDescriptor.GetDisplayName(argument);

            if (typeDescriptor is IAllowedValuesTypeDescriptor allowedValuesTypeDescriptor)
            {
                argument.AllowedValues = allowedValuesTypeDescriptor.GetAllowedValues(argument).ToList();
            }
            return argument;
        }

        private static IArgument BuildArgument(
            IArgumentDef argumentDef, 
            ExecutionConfig executionConfig, 
            object defaultValue,
            TypeInfo typeInfo)
        {
            if (argumentDef.ArgumentType == ArgumentType.Operand)
            {
                var operandAttr = argumentDef.Attributes.GetCustomAttribute<OperandAttribute>();
                return new Operand(operandAttr?.Name ?? argumentDef.Name)
                {
                    Description = operandAttr?.Description,
                    Arity = ArgumentArity.Default(argumentDef.Type, BooleanMode.Explicit),
                    DefaultValue = defaultValue,
                    TypeInfo = typeInfo
                };
            }

            var optionAttr = argumentDef.Attributes.GetCustomAttribute<OptionAttribute>();
            var argumentArity = ArgumentArity.Default(argumentDef.Type, GetOptionBooleanMode(argumentDef, executionConfig.AppSettings.BooleanMode, optionAttr));
            return new Option(
                new ArgumentTemplate(optionAttr?.Name ?? argumentDef.Name, optionAttr?.ShortName).ToString(),
                argumentArity)
            {
                Description = optionAttr?.Description,
                Inherited = optionAttr?.Inherited ?? false,
                DefaultValue = defaultValue,
                TypeInfo = typeInfo
            };
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