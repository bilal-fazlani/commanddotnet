using System.Reflection;

namespace CommandDotNet.Extensions
{
    internal static class MemberInfoExtensions
    {
        internal static string FullName(this MemberInfo memberInfo, bool includeNamespace = false) =>
            includeNamespace
                ? $"{memberInfo.DeclaringType?.FullName}.{memberInfo.Name}"
                : $"{memberInfo.DeclaringType?.Name}.{memberInfo.Name}";

        internal static string FullName(this ParameterInfo parameterInfo, bool includeNamespace = false) =>
            includeNamespace
                ? $"{parameterInfo.Member.DeclaringType?.FullName}.{parameterInfo.Member.Name}.{parameterInfo.Name}"
                : $"{parameterInfo.Member.DeclaringType?.Name}.{parameterInfo.Member.Name}.{parameterInfo.Name}";
    }
}