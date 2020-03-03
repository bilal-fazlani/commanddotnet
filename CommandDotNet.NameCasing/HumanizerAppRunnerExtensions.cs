using Humanizer;

namespace CommandDotNet.NameCasing
{
    public static class HumanizerAppRunnerExtensions
    {
        /// <summary>
        /// When true
        /// </summary>
        /// <param name="appRunner"></param>
        /// <param name="case">The case to apply</param>
        /// <param name="applyToNameOverrides">Case should be applied to names overridden in attributes.</param>
        /// <returns></returns>
        public static AppRunner UseNameCasing(this AppRunner appRunner, Case @case, bool applyToNameOverrides = false)
        {
            return applyToNameOverrides
                ? appRunner.Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType) =>
                    (nameOverride ?? memberName).ChangeCase(@case))
                : appRunner.Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType) =>
                    nameOverride ?? memberName.ChangeCase(@case));

        }

        private static string ChangeCase(this string value, Case @case)
        {
            switch (@case)
            {
                case Case.DontChange:
                    return value;
                case Case.CamelCase:
                    return value.Camelize();
                case Case.PascalCase:
                    return value.Dehumanize();
                case Case.KebabCase:
                    return value.Kebaberize();
                case Case.LowerCase:
                    return value.ToLowerInvariant();
                default:
                    return value;
            }
        }
        }
}
