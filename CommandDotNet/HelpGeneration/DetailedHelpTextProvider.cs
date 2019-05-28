using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    internal class DetailedHelpTextProvider : IHelpProvider
    {
        private readonly AppSettings _appSettings;

        public DetailedHelpTextProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        public string GetHelpText(ICommand app)
        {            
            var titleBuilder = BuildTitle(app);
            
            var usageBuilder = BuildUsage(app);

            var argumentsBuilder = BuildParameters(app, usageBuilder);

            var optionsBuilder = BuildOptions(app, usageBuilder);

            var commandsBuilder = BuildCommands(app, usageBuilder);

            var extendedHelpTextBuild = BuildExtendedHelpText(app);
            
            if (_appSettings.AllowArgumentSeparator)
            {
                usageBuilder.Append(" [[--] <arg>...]");
            }

            usageBuilder.AppendLine();

            return titleBuilder
                + usageBuilder.ToString()
                + argumentsBuilder
                + optionsBuilder
                + commandsBuilder
                + extendedHelpTextBuild;
        }

        private string BuildExtendedHelpText(ICommand app)
        {
            if (!string.IsNullOrEmpty(app.ExtendedHelpText))
            {
                return "\n" + app.ExtendedHelpText;
            }

            return null;
        }
        
        private StringBuilder BuildCommands(ICommand app, StringBuilder usageBuilder)
        {
            var commandsBuilder = new StringBuilder();
            var commands = app.Commands.Where(c => c.ShowInHelpText).ToList();
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
                
                if (app.OptionHelp != null)
                {
                    commandsBuilder.AppendFormat(
                        $"Use \"{app.GetFullCommandName()} [command] --{app.OptionHelp.LongName}\" for more information about a command.");
                    commandsBuilder.AppendLine();
                }
            }

            return commandsBuilder;
        }

        private  StringBuilder BuildOptions(ICommand app, StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            List<CommandOption> options = app.GetOptions().Where(o => o.ShowInHelpText).ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine($"Options:{Environment.NewLine}");

                foreach (var commandOption in options)
                {
                    //template format
                    optionsBuilder.Append(RenderParameter(options, commandOption, o =>
                    {
                        StringBuilder sb = new StringBuilder(o.Template);
                        sb.Append(o.Multiple ? " (Multiple)" : "");
                        return sb.ToString();
                    }));
                    
                    //type display name
                    optionsBuilder.Append(RenderParameter(options, commandOption, o =>
                    {
                        if(!string.IsNullOrEmpty(o.TypeDisplayName))
                            return $"<{o.TypeDisplayName.ToUpperInvariant()}>";
                        return null;
                    }));
                    
                    //default value
                    optionsBuilder.Append(RenderParameter(options, commandOption, o => 
                        o.DefaultValue.IsNullValue() 
                            ? null 
                            : $"[{o.DefaultValue.ToString()}]"));

                    //description
                    optionsBuilder.Append(string.IsNullOrEmpty(commandOption.Description)? null: $"{Environment.NewLine}  {commandOption.Description}");
                    
                    //Allowed values
                    optionsBuilder.Append(
                        (!string.IsNullOrEmpty(commandOption.TypeDisplayName) && (commandOption.AllowedValues?.Any() ?? false))
                            ? $"{Environment.NewLine}  Allowed values: {string.Join(", ", commandOption.AllowedValues)}"
                            : null);
                                        
                    optionsBuilder.AppendLine();
                    optionsBuilder.AppendLine();
                }
            }

            return optionsBuilder;
        }
        
        private StringBuilder BuildParameters(ICommand app, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = app.Arguments.Where(a => a.ShowInHelpText).ToList();
                        
            if (arguments.Any())
            {
                usageBuilder.Append(" [arguments]");
                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine($"Arguments:{Environment.NewLine}");

                foreach (var commandArgument in arguments)
                {
                    //template format
                    argumentsBuilder.Append(RenderParameter(arguments, commandArgument, arg =>
                    {
                        StringBuilder sb = new StringBuilder(arg.Name);
                        sb.Append(arg.MultipleValues ? " (Multiple)" : "");
                        return sb.ToString();
                    }));
                    
                    //type display name
                    argumentsBuilder.Append(RenderParameter(arguments, commandArgument, arg =>
                    {
                        if(!string.IsNullOrEmpty(arg.TypeDisplayName))
                            return $"<{arg.TypeDisplayName.ToUpperInvariant()}>";
                        return null;
                    }));
                    
                    //default value
                    argumentsBuilder.Append(RenderParameter(arguments, commandArgument, arg =>
                        arg.DefaultValue.IsNullValue()
                            ? null
                            : $"[{arg.DefaultValue.ToString()}]"));

                    //description
                    argumentsBuilder.Append(string.IsNullOrEmpty(commandArgument.Description) ? null : $"{Environment.NewLine}  {commandArgument.Description}");
                    
                    //Allowed values
                    argumentsBuilder.Append(
                        (!string.IsNullOrEmpty(commandArgument.TypeDisplayName) && (commandArgument.AllowedValues?.Any() ?? false))
                            ? $"{Environment.NewLine}  Allowed values: {string.Join(", ", commandArgument.AllowedValues)}"
                            : null);
                        
                    argumentsBuilder.AppendLine();
                    argumentsBuilder.AppendLine();
                }
            }

            return argumentsBuilder;
        }

        private StringBuilder BuildUsage(ICommand app)
        {
            var usageBuilder = new StringBuilder("Usage: ");
            usageBuilder.Append(app.GetFullCommandName());
            return usageBuilder;
        }

        private StringBuilder BuildTitle(ICommand app)
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(app.Description))
                titleBuilder.AppendLine(app.Description + Environment.NewLine);
            return titleBuilder;
        }

        private string RenderParameter<T>(List<T> arguments, T argument, Func<T, string> function)
        {
            int maxLength = GetColumnMaxLength(arguments, function);
            return string.Format(ColumnFormat(maxLength), function(argument));
        }
        
        private string ColumnFormat(int maxLength) => $"  {{0, -{maxLength + 2}}}";
        
        private int GetColumnMaxLength<T>(List<T> options, Func<T,string> column)
        {
            return options.Max(o => column(o)?.Length ?? 0);
        }
    }
}