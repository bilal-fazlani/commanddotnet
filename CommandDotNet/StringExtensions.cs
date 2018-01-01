using CommandDotNet.Models;
using Humanizer;

namespace CommandDotNet
{
    public static class StringExtensions
    {
        public static string ChangeCase(this string value, Case @case)
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