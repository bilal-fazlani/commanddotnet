using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;

namespace CommandDotNet.Help
{
    internal class DetailedHelpTextProvider : IHelpProvider
    {
        private readonly AppSettings _appSettings;

        public DetailedHelpTextProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        public string GetHelpText(ICommand command)
        {            
            var titleBuilder = BuildTitle(command);
            
            var usageBuilder = BuildUsage(command);

            var commandsBuilder = BuildCommands(command, usageBuilder);
            
            var operandsBuilder = BuildOperands(command, usageBuilder);

            var optionsBuilder = BuildOptions(command, usageBuilder);

            var extendedHelpTextBuild = BuildExtendedHelpText(command);
            
            if (_appSettings.AllowArgumentSeparator)
            {
                usageBuilder.Append(" [[--] <arg>...]");
            }

            usageBuilder.AppendLine();

            return titleBuilder
                + usageBuilder.ToString()
                + operandsBuilder
                + optionsBuilder
                + commandsBuilder
                + extendedHelpTextBuild;
        }

        private StringBuilder BuildUsage(ICommand command)
        {
            return new StringBuilder("Usage: ").AppendUsageCommandNames(command, _appSettings);
        }

        private StringBuilder BuildTitle(ICommand command)
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(command.Description))
                titleBuilder.AppendLine(command.Description + Environment.NewLine);
            return titleBuilder;
        }

        private string BuildExtendedHelpText(ICommand command)
        {
            if (!string.IsNullOrEmpty(command.ExtendedHelpText))
            {
                return "\n" + command.ExtendedHelpText;
            }

            return null;
        }

        private StringBuilder BuildCommands(ICommand command, StringBuilder usageBuilder)
        {
            var commandsBuilder = new StringBuilder();
            var commands = command.Commands.ToList();
            if (commands.Any())
            {
                usageBuilder.Append(" [command]");

                commandsBuilder.AppendLine();
                commandsBuilder.AppendLine($"Commands:{Environment.NewLine}");
                var maxCmdLen = commands.Max(c => c.Name.Length);
                var outputFormat = $"  {{0, -{maxCmdLen + 2}}}{{1}}";
                               
                foreach (var cmd in commands.OrderBy(c => c.Name))
                {
                    commandsBuilder.AppendFormat(outputFormat, cmd.Name, cmd.Description);
                    commandsBuilder.AppendLine();
                }

                commandsBuilder.AppendLine();

                commandsBuilder.Append("Use \"");
                commandsBuilder.AppendUsageCommandNames(command, _appSettings);
                commandsBuilder.AppendLine($" [command] --{Constants.HelpArgumentTemplate.Name}\" for more information about a command.");
            }

            return commandsBuilder;
        }

        private  StringBuilder BuildOptions(ICommand command, StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            List<IOption> options = command.GetOptions()
                .Where(o => _appSettings.Help.PrintHelpOption || o.Name != Constants.HelpArgumentTemplate.Name)
                .OrderBy(o => o.IsSystemOption)
                .ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine($"Options:{Environment.NewLine}");

                var helpValues = options.Select(a =>
                {
                    var allowedValues = a.TypeInfo.DisplayName.IsNullOrEmpty() || a.AllowedValues == null || !a.AllowedValues.Any()
                        ? null
                        : $"{Environment.NewLine}  Allowed values: {a.AllowedValues.ToCsv(", ")}";

                    return new ArgumentHelpValues
                    {
                        Template = $"{a.Template}{(a.Arity.AllowsZeroOrMore() ? " (Multiple)" : "")}",
                        DisplayName = a.TypeInfo.DisplayName.IsNullOrEmpty()
                            ? null
                            : $"<{a.TypeInfo.DisplayName.ToUpperInvariant()}>",
                        DefaultValue = GetDefaultValueString(a.DefaultValue),
                        Description = a.Description.IsNullOrEmpty()
                            ? null
                            : $"{Environment.NewLine}  {a.Description}",
                        AllowedValues = allowedValues
                    };
                }).ToList();

                AppendArguments(optionsBuilder, helpValues);
            }

            return optionsBuilder;
        }

        private StringBuilder BuildOperands(ICommand command, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = command.Operands.ToList();
                        
            if (arguments.Any())
            {
                // standard term for these in help of other tools is arguments
                usageBuilder.Append(" [arguments]");
                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine($"Arguments:{Environment.NewLine}");
                
                var helpValues = arguments.Select(a =>
                    new ArgumentHelpValues
                    {
                        Template = $"{a.Name}{(a.Arity.AllowsZeroOrMore() ? " (Multiple)" : "")}",
                        DisplayName = a.TypeInfo.DisplayName.IsNullOrEmpty()
                            ? null
                            : $"<{a.TypeInfo.DisplayName.ToUpperInvariant()}>",
                        DefaultValue = GetDefaultValueString(a.DefaultValue),
                        Description = a.Description.IsNullOrEmpty()
                            ? null
                            : $"{Environment.NewLine}  {a.Description}",
                        AllowedValues = a.TypeInfo.DisplayName.IsNullOrEmpty() || a.AllowedValues == null || !a.AllowedValues.Any()
                            ? null
                            : $"{Environment.NewLine}  Allowed values: {a.AllowedValues.ToCsv(", ")}"
                    }).ToList();

                AppendArguments(argumentsBuilder, helpValues);
            }

            return argumentsBuilder;
        }

        private void AppendArguments(StringBuilder argumentsBuilder, List<ArgumentHelpValues> helpValues)
        {
            var templateMaxLength = helpValues.Max(a => a.Template?.Length) ?? 0;
            var displayNameMaxLength = helpValues.Max(a => a.DisplayName?.Length) ?? 0;
            var defaultValueMaxLength = helpValues.Max(a => a.DefaultValue?.Length) ?? 0;

            foreach (var helpValue in helpValues)
            {
                argumentsBuilder.Append(Align(templateMaxLength, helpValue.Template));
                argumentsBuilder.Append(Align(displayNameMaxLength, helpValue.DisplayName));
                argumentsBuilder.Append(Align(defaultValueMaxLength, helpValue.DefaultValue));
                argumentsBuilder.Append(helpValue.Description);
                argumentsBuilder.Append(helpValue.AllowedValues);
                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine();
            }
        }

        private class ArgumentHelpValues
        {
            public string Template;
            public string DisplayName;
            public string DefaultValue;
            public string Description;
            public string AllowedValues;
        }

        private static string GetDefaultValueString(object defaultValue)
        {
            if (defaultValue.IsNullValue())
            {
                return null;
            }

            if (defaultValue is string)
            {
                return $"[{defaultValue}]";
            }

            if (defaultValue is IEnumerable collection)
            {
                return $"[{collection.ToCsv()}]";
            }

            return $"[{defaultValue}]";
        }

        private string Align(int maxLength, string value) => string.Format($"  {{0, -{maxLength + 2}}}", value);
    }
}