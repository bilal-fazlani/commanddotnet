using System;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    /// <summary>
    /// An argument template is a terse description of an arguments short name, long name and type display name.
    /// e.g. "-d|--dryrun"
    /// </summary>
    public class ArgumentTemplate
    {
        public string LongName { get; }
        public char? ShortName { get; }
        public string TypeDisplayName { get; }

        public ArgumentTemplate(
            string longName = null,
            char? shortName = null,
            string shortNameAsString = null,
            string typeDisplayName = null)
        {
            LongName = longName;
            ShortName = shortName;
            TypeDisplayName = typeDisplayName;

            if (shortName.IsNullOrWhitespace() && !shortNameAsString.IsNullOrWhitespace())
            {
                if (shortNameAsString.Length > 1)
                {
                    throw new ArgumentException($"Short name must be a single character: {shortNameAsString}", nameof(shortNameAsString));
                }

                ShortName = shortNameAsString.Single();
            }

            if (LongName.IsNullOrWhitespace() && ShortName.IsNullOrWhitespace())
            {
                throw new ArgumentException("a long or short name is required");
            }
        }

        public static ArgumentTemplate Parse(string template)
        {
            string longName = null, shortName = null, typeDisplayName = null;

            foreach (var part in template.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                {
                    longName = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    shortName = part.Substring(1);
                }
                else if (part.StartsWith("<") && part.EndsWith(">"))
                {
                    typeDisplayName = part.Substring(1, part.Length - 2);
                }
                else
                {
                    throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
                }
            }

            try
            {

                return new ArgumentTemplate(longName: longName, shortNameAsString: shortName, typeDisplayName: typeDisplayName);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid template pattern '{template}' {e.Message}. pattern: -short|--long", nameof(template), e);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            void AppendIfNotNull(string prefix, string value)
            {
                if (value.IsNullOrWhitespace()) return;
                if (sb.Length > 0) sb.Append("|");
                sb.Append(prefix);
                sb.Append(value);
            }

            AppendIfNotNull("-", ShortName?.ToString());
            AppendIfNotNull("--", LongName);

            if (sb.Length == 0)
            {
                throw new Exception("Unable to generate template. " +
                                    $"One of either {nameof(LongName)} or {nameof(ShortName)} must be specified");
            }

            return TypeDisplayName.IsNullOrWhitespace()
                ? sb.ToString()
                : $"{sb} <{TypeDisplayName}>";
        }
    }
}