using System.Linq;
using System.Text;
using CommandDotNet.MicrosoftCommandLineUtils;
using ConsoleTables;

namespace CommandDotNet.HelpGeneration
{
    public class TabularHelpTextGenerator : IHelpGenerator
    {
        public string GetHelpText(CommandLineApplication app)
        {            
            var titleBuilder = BuildTitle(app);
            
            var usageBuilder = BuildUsage(app);

            var argumentsBuilder = BuildArguments(app, usageBuilder);

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
                
                var table = new ConsoleTable("Command", "Description");
                
                foreach (var cmd in commands.OrderBy(c => c.Name))
                {
                    table.AddRow(cmd.Name, cmd.Description);
                }

                commandsBuilder.Append(table.ToStringAlternative());
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
            var options = app.GetOptions().Where(o => o.ShowInHelpText).ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine("Options:");
                
                var table = new ConsoleTable("Option", "Type" ,"Description", "Default", "Multiple");
                
                foreach (var opt in options)
                {
                    table.AddRow(opt.Template, opt.TypeDisplayName ,opt.Description, opt.DefaultValue, opt.Multiple? "✓" : null);
                }
                optionsBuilder.Append(table.ToStringAlternative());
            }

            return optionsBuilder;
        }

        private StringBuilder BuildArguments(CommandLineApplication app, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = app.Arguments.Where(a => a.ShowInHelpText).ToList();
                        
            if (arguments.Any())
            {
                usageBuilder.Append(" [arguments]");
                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine("Arguments:");
                
                var table = new ConsoleTable("Argumment", "Type", "Description", "Default", "Multiple");

                foreach (var arg in arguments)
                {
                    table.AddRow(arg.Name, arg.TypeDisplayName, arg.Description, arg.DefaultValue, arg.MultipleValues? "✓" : null);
                }

                argumentsBuilder.Append(table.ToStringAlternative());
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
    }
}