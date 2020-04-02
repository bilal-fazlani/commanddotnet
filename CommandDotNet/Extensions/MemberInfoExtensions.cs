using System.Reflection;

namespace CommandDotNet.Extensions
{
    internal static class MemberInfoExtensions
    {
        internal static string FullName(this MemberInfo memberInfo, bool includeNamespace = false)
        {
            string typeName = null;
            if (includeNamespace)
            {
                typeName = memberInfo.DeclaringType?.FullName;
            }
            else if (memberInfo.DeclaringType != null)
            {
                typeName = memberInfo.DeclaringType.IsNested
                    ? $"{memberInfo.DeclaringType.DeclaringType?.Name}.{memberInfo.DeclaringType.Name}"
                    : memberInfo.DeclaringType.Name;
            }
            return $"{typeName}.{memberInfo.Name}";
        }

        internal static string FullName(this ParameterInfo parameterInfo, bool includeNamespace = false) =>
            includeNamespace
                ? $"{parameterInfo.Member.DeclaringType?.FullName}.{parameterInfo.Member.Name}.{parameterInfo.Name}"
                : $"{parameterInfo.Member.DeclaringType?.Name}.{parameterInfo.Member.Name}.{parameterInfo.Name}";
    }
}