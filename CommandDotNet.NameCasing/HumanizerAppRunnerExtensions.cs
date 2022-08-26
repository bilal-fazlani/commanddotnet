using Humanizer;

namespace CommandDotNet.NameCasing
{
    public static class HumanizerAppRunnerExtensions
    {
        // begin-snippet: name_casing_transformation
        /// <summary>Change the case of argument and command names to match the given cases</summary>
        /// <param name="appRunner"></param>
        /// <param name="case">The case to apply</param>
        /// <param name="applyToNameOverrides">Case should be applied to names overridden in attributes.</param>
        public static AppRunner UseNameCasing(this AppRunner appRunner, Case @case, bool applyToNameOverrides = false)
        {
            appRunner.Configure(b => b.Services.Add(new CaseChanger(s => ChangeCase(s, @case))));
            return applyToNameOverrides
                ? appRunner.Configure(b => b.NameTransformation = (_, memberName, nameOverride, _) =>
                    (nameOverride ?? memberName).ChangeCase(@case))
                : appRunner.Configure(b => b.NameTransformation = (_, memberName, nameOverride, _) =>
                    nameOverride ?? memberName.ChangeCase(@case));
        }
        // end-snippet

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
                case Case.SnakeCase:
                    return value.Underscore();
                default:
                    return value;
            }
        }
    }
}
