using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandDotNet.Help;

internal class BasicHelpTextProvider(AppSettings appSettings) : HelpTextProvider(appSettings)
{
    protected override string FormatSectionHeader(string header)
        => "usage".Equals(header, StringComparison.OrdinalIgnoreCase)
            ? $"{header}:"
            : $"{header}:{Environment.NewLine}";

    protected override string SectionArguments<T>(Command command, ICollection<T> arguments)
    {
        // Check if we're dealing with options (which support grouping)
        var options = arguments.OfType<Option>().ToList();
        if (options.Count == arguments.Count)
        {
            return SectionGroupedOptionsBasic(options) ?? string.Empty;
        }

        // For operands or mixed types, use the original behavior
        var nameMaxLength = arguments.Max(a => Name(a)?.Length) ?? 0;

        var sb = new StringBuilder();
        foreach (var argument in arguments)
        {
            sb.AppendLine(Row((nameMaxLength, Name(argument)), (-1, argument.Description)));
        }
        return sb.ToString();

        string? Name(IArgument arg) => ArgumentName(arg);
    }

    private string? SectionGroupedOptionsBasic(ICollection<Option> options)
    {
        if (!options.Any())
        {
            return null;
        }

        // Group options by their Group property
        var ungrouped = options.Where(o => string.IsNullOrWhiteSpace(o.Group)).ToList();
        var grouped = options.Where(o => !string.IsNullOrWhiteSpace(o.Group))
            .GroupBy(o => o.Group!)
            .OrderBy(g => g.Key)
            .ToList();

        var sb = new StringBuilder();

        // Display ungrouped options first
        if (ungrouped.Any())
        {
            var nameMaxLength = ungrouped.Max(o => ArgumentName(o)?.Length) ?? 0;
            foreach (var option in ungrouped)
            {
                sb.AppendLine(Row((nameMaxLength, ArgumentName(option)), (-1, option.Description)));
            }
        }

        // Display grouped options alphabetically by group name
        foreach (var group in grouped)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            sb.AppendLine(GroupHeader(group.Key));
            sb.AppendLine();

            var groupOptions = group.ToList();
            var nameMaxLength = groupOptions.Max(o => ArgumentName(o)?.Length) ?? 0;
            foreach (var option in groupOptions)
            {
                sb.AppendLine(Row((nameMaxLength, ArgumentName(option)), (-1, option.Description)));
            }
        }

        return sb.ToString();
    }
}