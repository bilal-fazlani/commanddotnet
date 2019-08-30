using System.Reflection;

namespace CommandDotNet.Extensions
{
    internal static class MemberInfoExtensions
    {
        internal static string FullName(this MemberInfo memberInfo, bool includeNamespace = false) =>
            includeNamespace
                ? $"{memberInfo.DeclaringType?.FullName}.{memberInfo.Name}"
                : $"{memberInfo.DeclaringType?.Name}.{memberInfo.Name}";
    }
}