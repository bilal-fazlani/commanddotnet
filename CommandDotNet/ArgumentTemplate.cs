using System;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class ArgumentTemplate
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string SymbolName { get; set; }
        public string TypeDisplayName { get; set; }

        public ArgumentTemplate()
        {
        }

        public ArgumentTemplate(string template)
        {
            foreach (var part in template.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                {
                    LongName = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    var optName = part.Substring(1);

                    // If there is only one char and it is not an English letter, it is a symbol option (e.g. "-?")
                    if (optName.Length == 1 && !IsEnglishLetter(optName[0]))
                    {
                        SymbolName = optName;
                    }
                    else
                    {
                        ShortName = optName;
                    }
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

            if (string.IsNullOrEmpty(LongName) && string.IsNullOrEmpty(ShortName) && string.IsNullOrEmpty(SymbolName))
            {
                throw new ArgumentException($"Invalid template pattern '{template}' Unable to determine the name of the argument. " +
                                            $"provide either a symbol, short or long name following this pattern: -symbol|-short|--long", nameof(template));
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
            AppendIfNotNull("-", SymbolName);
            AppendIfNotNull("--", LongName);

            if (sb.Length == 0)
            {
                throw new Exception("Unable to generate template. " +
                                    $"One of either {nameof(LongName)}, {nameof(ShortName)} or {nameof(SymbolName)} must be specified");
            }

            return TypeDisplayName.IsNullOrWhitespace()
                ? sb.ToString()
                : $"{sb} <{TypeDisplayName}>";
        }
    }
}