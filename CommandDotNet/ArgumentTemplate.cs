using System;
using System.Text;

namespace CommandDotNet
{
    /// <summary>
    /// An argument template is a terse description of an arguments short and/or long name.
    /// e.g. "-u | --username"
    /// </summary>
    public static class ArgumentTemplate
    {
        public static string Build(string longName, char? shortName)
        {
            var sb = new StringBuilder();

            void AppendIfNotNull(string prefix, string value)
            {
                if (value.IsNullOrWhitespace()) return;
                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(prefix);
                sb.Append(value);
            }

            AppendIfNotNull("-", shortName?.ToString());
            AppendIfNotNull("--", longName);

            if (sb.Length == 0)
            {
                throw new Exception("Unable to generate template. " +
                                    $"One of either {nameof(longName)} or {nameof(shortName)} must be specified");
            }

            return sb.ToString();
        }
    }
}