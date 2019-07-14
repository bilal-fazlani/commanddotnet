using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class DefinitionReflectionExtensions
    {
        internal static string BuildName(this ParameterInfo parameterInfo, ExecutionConfig executionConfig)
        {
            return parameterInfo.GetCustomAttributes().OfType<INameAndDescription>().FirstOrDefault()?.Name
                   ?? parameterInfo.Name.ChangeCase(executionConfig.AppSettings.Case);
        }

        internal static string BuildName(this MemberInfo memberInfo, ExecutionConfig executionConfig)
        {
            var nameFromAttr = memberInfo.GetCustomAttributes().OfType<INameAndDescription>()?.FirstOrDefault()?.Name;
            
            var appSettings = executionConfig.AppSettings;
            var acase = appSettings.Case;

            var nameFromMethod = memberInfo.Name.ChangeCase(acase);
            return nameFromAttr ?? nameFromMethod;
        }

        internal static bool IsOption(this ICustomAttributeProvider attributeProvider, ArgumentMode argumentMode)
        {
            // If developer defined the mode with an attribute, use that,
            // otherwise use the defined ArgumentMode

            switch (argumentMode)
            {
                case ArgumentMode.Operand:
                    return attributeProvider.HasAttribute<OptionAttribute>();
                case ArgumentMode.Option:
                    return !attributeProvider.HasAttribute<OperandAttribute>()
                           && !attributeProvider.HasAttribute<ArgumentAttribute>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(argumentMode), argumentMode, null);
            }
        }
    }
}