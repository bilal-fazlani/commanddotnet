using System.Linq;
using System.Text;

namespace CommandDotNet.Help
{
    internal class BasicHelpTextProvider : IHelpProvider
    {
        private readonly AppSettings _appSettings;

        public BasicHelpTextProvider(AppSettings appSettings)
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
                commandsBuilder.AppendLine("Commands:");
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
            var options = command.GetOptions()
                .OrderBy(o => o.IsSystemOption)
                .ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine("Options:");
                                
                var maxOptLen = options.Max(o => o.Template.Length);
                var outputFormat = $"  {{0, -{maxOptLen + 2}}}{{1}}";
                
                foreach (var opt in options)
                {
                    optionsBuilder.AppendFormat(outputFormat, opt.Template, opt.Description);
                    optionsBuilder.AppendLine();    
                }
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
                argumentsBuilder.AppendLine("Arguments:");
                                
                var maxArgLen = arguments.Max(a => a.Name.Length);
                var outputFormat = $"  {{0, -{maxArgLen + 2}}}{{1}}";
                foreach (var arg in arguments)
                {
                    argumentsBuilder.AppendFormat(outputFormat, arg.Name, arg.Description);
                    argumentsBuilder.AppendLine();
                }
            }

            return argumentsBuilder;
        }

        private StringBuilder BuildUsage(ICommand command)
        {
            return new StringBuilder("Usage: ").AppendUsageCommandNames(command, _appSettings);
        }

        private StringBuilder BuildTitle(ICommand command)
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(command.Description))
                titleBuilder.AppendLine(command.Description + "\n");
            return titleBuilder;
        }
    }
}