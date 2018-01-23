using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.HelpGeneration
{
    public class StandardHelpTextGenerator : IHelpGenerator
    {
        public string GetHelpText(CommandLineApplication app)
        {            
            var titleBuilder = BuildTitle(app);
            
            var usageBuilder = BuildUsage(app);

            var argumentsBuilder = BuildParameters(app, usageBuilder);

            var optionsBuilder = BuildOptions(app, usageBuilder);

            var commandsBuilder = BuildCommands(app, usageBuilder);

            var extendedHelpTextBuild = BuildExtendedHelpText(app);
            
            if (app.AllowArgumentSeparator)
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

        private string BuildExtendedHelpText(CommandLineApplication app)
        {
            if (!string.IsNullOrEmpty(app.ExtendedHelpText))
            {
                return "\n" + app.ExtendedHelpText;
            }

            return null;
        }
        
        private StringBuilder BuildCommands(CommandLineApplication app, StringBuilder usageBuilder)
        {
            var commandsBuilder = new StringBuilder();
            var commands = app.Commands.Where(c => c.ShowInHelpText).ToList();
            if (commands.Any())
            {
                usageBuilder.Append(" [command]");

                commandsBuilder.AppendLine();
                commandsBuilder.AppendLine("Commands:");
               
                RenderList(commands, commandsBuilder, 
                    command => command.Name,
                    command => command.Description);
                
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

        private  StringBuilder BuildOptions(CommandLineApplication app, StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            List<CommandOption> options = app.GetOptions().Where(o => o.ShowInHelpText).ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine("Options:");

                RenderList(options, optionsBuilder, 
                    opt => {
                        StringBuilder sb = new StringBuilder(opt.Template);
                        if (!string.IsNullOrEmpty(opt.TypeDisplayName))
                        {
                            sb.Append($" [{opt.TypeDisplayName}]");
                        }
                        return sb.ToString();
                    },
                    opt => opt.DefaultValue != DBNull.Value ? $"Default: {opt.DefaultValue}" : null,
                    opt => opt.Description);
            }

            return optionsBuilder;
        }
        
        private StringBuilder BuildParameters(CommandLineApplication app, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = app.Arguments.Where(a => a.ShowInHelpText).ToList();
                        
            if (arguments.Any())
            {
                usageBuilder.Append(" [arguments]");
                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine("Arguments:");
                
                RenderList(arguments, argumentsBuilder, 
                    arg =>
                    {
                        StringBuilder sb = new StringBuilder(arg.Name);
                        if (!string.IsNullOrEmpty(arg.TypeDisplayName))
                        {
                            sb.Append($" [{arg.TypeDisplayName}]");
                        }
                        return sb.ToString();
                    },
                    arg => arg.DefaultValue != DBNull.Value ? $"Default: {arg.DefaultValue}" : null,
                    arg => arg.Description);
            }

            return argumentsBuilder;
        }

        private StringBuilder BuildUsage(CommandLineApplication app)
        {
            var usageBuilder = new StringBuilder("Usage: ");
            usageBuilder.Append(app.GetFullCommandName());
            return usageBuilder;
        }

        private StringBuilder BuildTitle(CommandLineApplication app)
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(app.Description))
                titleBuilder.AppendLine(app.Description + "\n");
            return titleBuilder;
        }
        
        private void RenderList<T>(List<T> records, StringBuilder stringBuilder, params Func<T, string>[] fields)
        {
            int totalMaxLength = 0;
            
            foreach (var field in fields)
            {
                totalMaxLength += GetColumnMaxLength(records, field);
            }

            totalMaxLength += 4 * fields.Length;
            
            string dashes = string.Join("",Enumerable.Range(0, totalMaxLength).Select(i => "-"));

            stringBuilder.AppendLine(dashes);
            
            foreach (var record in records)
            {
                foreach (var field in fields)
                {
                    stringBuilder.Append(RenderParameter(records, record, field));
                }

                stringBuilder.AppendLine();
                stringBuilder.AppendLine(dashes);
            }
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