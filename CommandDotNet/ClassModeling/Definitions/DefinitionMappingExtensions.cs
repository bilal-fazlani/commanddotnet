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
            var command = new Command(commandDef.Name, commandDef.CustomAttributeProvider, parent);
            command.ContextData.Set(commandDef);
            commandDef.Command = command;

            var metadataAttribute = commandDef.CustomAttributeProvider.GetCustomAttribute<ApplicationMetadataAttribute>();
            if (metadataAttribute != null)
            {
                command.Description = metadataAttribute.Description;
                command.ExtendedHelpText = metadataAttribute.ExtendedHelpText;
            }

            var commandBuilder = new CommandBuilder(command);

            commandDef.Arguments
                .Select(a => a.ToArgument(commandContext.AppConfig))
                .ForEach(commandBuilder.AddArgument);

            if (commandDef.IsExecutable)
            {
                commandDef.MiddlewareMethodDef.ArgumentDefs
                    .Select(d => d.Argument)
                    .OfType<Option>()
                    .Where(o => o.Inherited)
                    .ForEach(commandBuilder.AddArgument);
            }

            commandContext.AppConfig.BuildEvents.CommandCreated(commandContext, commandBuilder);

            commandDef.SubCommands
                .Select(c => c.ToCommand(command, commandContext).Command)
                .ForEach(commandBuilder.AddSubCommand);
            return commandBuilder;
        }

        private static IArgument ToArgument(this IArgumentDef argumentDef, AppConfig appConfig)
        {
            var underlyingType = argumentDef.Type.GetUnderlyingType();

            var argument = BuildArgument(
                argumentDef, 
                appConfig, 
                argumentDef.HasDefaultValue ? argumentDef.DefaultValue : null,
                new TypeInfo
                {
                    Type = argumentDef.Type,
                    UnderlyingType = underlyingType,
                }
            );
            argumentDef.Argument = argument;

            var typeDescriptor = appConfig.AppSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(underlyingType);
            argument.TypeInfo.DisplayName = typeDescriptor.GetDisplayName(argument);

            if (typeDescriptor is IAllowedValuesTypeDescriptor allowedValuesTypeDescriptor)
            {
                argument.AllowedValues = allowedValuesTypeDescriptor.GetAllowedValues(argument).ToList();
            }
            return argument;
        }

        private static IArgument BuildArgument(
            IArgumentDef argumentDef, 
            AppConfig appConfig, 
            object defaultValue,
            TypeInfo typeInfo)
        {
            if (argumentDef.ArgumentType == ArgumentType.Operand)
            {
                var operandAttr = argumentDef.Attributes.GetCustomAttribute<OperandAttribute>() 
                                  ?? (INameAndDescription) argumentDef.Attributes.GetCustomAttribute<ArgumentAttribute>();
                return new Operand(operandAttr?.Name ?? argumentDef.Name, argumentDef.Attributes)
                {
                    Description = operandAttr?.Description,
                    Arity = ArgumentArity.Default(argumentDef.Type, BooleanMode.Explicit),
                    DefaultValue = defaultValue,
                    TypeInfo = typeInfo
                };
            }

            var optionAttr = argumentDef.Attributes.GetCustomAttribute<OptionAttribute>();
            var argumentArity = ArgumentArity.Default(argumentDef.Type, GetOptionBooleanMode(argumentDef, appConfig.AppSettings.BooleanMode, optionAttr));
            return new Option(
                new ArgumentTemplate(optionAttr?.LongName ?? argumentDef.Name, optionAttr?.ShortName).ToString(),
                argumentArity, customAttributeProvider: argumentDef.Attributes)
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