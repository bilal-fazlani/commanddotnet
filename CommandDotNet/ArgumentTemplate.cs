using System;
using System.Text;

namespace CommandDotNet
{
    public class ArgumentTemplate
    {
        public string Name { get; }
        public string ShortName { get; }
        public string TypeDisplayName { get; }

        public ArgumentTemplate(
            string name = null, 
            string shortName = null,
            string typeDisplayName = null)
        {
            Name = name;
            ShortName = shortName;
            TypeDisplayName = typeDisplayName;
        }

        public ArgumentTemplate(string template)
        {
            foreach (var part in template.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                {
                    Name = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    var optName = part.Substring(1);
                    ShortName = optName;
                }
                else if (part.StartsWith("<") && part.EndsWith(">"))
                {
                    TypeDisplayName = part.Substring(1, part.Length - 2);
                }
                else
                {
                    throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
                }
            }

            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(ShortName))
            {
                throw new ArgumentException($"Invalid template pattern '{template}' Unable to determine the name of the argument. " +
                                            "provide either a symbol, short or long name following this pattern: -symbol|-short|--long", nameof(template));
            }
        }

        private bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
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

            AppendIfNotNull("-", ShortName);
            AppendIfNotNull("--", Name);

            if (sb.Length == 0)
            {
                throw new Exception("Unable to generate template. " +
                                    $"One of either {nameof(Name)} or {nameof(ShortName)} must be specified");
            }

            return TypeDisplayName.IsNullOrWhitespace()
                ? sb.ToString()
                : $"{sb} <{TypeDisplayName}>";
        }
    }
}