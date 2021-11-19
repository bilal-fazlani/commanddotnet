using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class DefinitionReflectionExtensions
    {
        internal static string BuildName(this ParameterInfo parameterInfo, CommandNodeType commandNodeType, AppConfig appConfig)
        {
            if (parameterInfo.Name is null)
            {
                throw new InvalidConfigurationException(
                    $"{nameof(parameterInfo)}.{nameof(parameterInfo.Name)} cannot be null. host:{parameterInfo.Member} position:{parameterInfo.Position}");
            }
            return BuildName(parameterInfo, parameterInfo.Name, commandNodeType, appConfig);
        }

        internal static string BuildName(this MemberInfo memberInfo, CommandNodeType commandNodeType, AppConfig appConfig, string? overrideName = null)
        {
            return BuildName(memberInfo, memberInfo.Name, commandNodeType, appConfig, overrideName);
        }

        private static string BuildName(ICustomAttributeProvider attributes, string memberName, CommandNodeType commandNodeType, AppConfig appConfig, string? overrideName = null)
        {
            overrideName ??= attributes.GetCustomAttributes(true).OfType<INameAndDescription>().FirstOrDefault()?.Name;
            return appConfig.NameTransformation(attributes, memberName, overrideName, commandNodeType);
        }

        internal static bool IsOption(this ICustomAttributeProvider attributeProvider, ArgumentMode argumentMode)
        {
            // If developer defined the mode with an attribute, use that,
            // otherwise use the defined ArgumentMode

            return argumentMode switch
            {
                ArgumentMode.Operand => attributeProvider.HasAttribute<OptionAttribute>(),
                ArgumentMode.Option => !attributeProvider.HasAttribute<OperandAttribute>(),
                _ => throw new ArgumentOutOfRangeException(nameof(argumentMode), argumentMode, null)
            };
        }
    }
}