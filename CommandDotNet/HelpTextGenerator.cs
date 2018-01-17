using System;
using System.Linq;
using System.Text;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet
{
    public class HelpTextGenerator
    {
        private readonly CommandLineApplication _app;

        public HelpTextGenerator(CommandLineApplication app)
        {
            _app = app;
        }

        public string GetHelpText()
        {            
            var titleBuilder = BuildTitle();
            
            var usageBuilder = BuildUsage();

            var argumentsBuilder = BuildArguments(usageBuilder);

            var optionsBuilder = BuildOptions(usageBuilder);

            var commandsBuilder = BuildCommands(usageBuilder);

            if (_app.AllowArgumentSeparator)
            {
                usageBuilder.Append(" [[--] <arg>...]");
            }

            usageBuilder.AppendLine();

            return titleBuilder
                + usageBuilder.ToString()
                + argumentsBuilder
                + optionsBuilder
                + commandsBuilder
                + _app.ExtendedHelpText;
        }
        
        private StringBuilder BuildCommands(StringBuilder usageBuilder)
        {
            var commandsBuilder = new StringBuilder();
            var commands = _app.Commands.Where(c => c.ShowInHelpText).ToList();
            if (commands.Any())
            {
                usageBuilder.Append(" [command]");

                commandsBuilder.AppendLine();
                commandsBuilder.AppendLine("Commands:");
                var maxCmdLen = commands.Max(c => c.Name.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxCmdLen + 2);
                foreach (var cmd in commands.OrderBy(c => c.Name))
                {
                    commandsBuilder.AppendFormat(outputFormat, cmd.Name, cmd.Description);
                    commandsBuilder.AppendLine();
                }

                if (_app.OptionHelp != null)
                {
                    commandsBuilder.AppendLine();
                    commandsBuilder.AppendFormat(
                        $"Use \"{_app.GetFullCommandName()} [command] --{_app.OptionHelp.LongName}\" for more information about a command.");
                    commandsBuilder.AppendLine();
                }
            }

            return commandsBuilder;
        }

        private  StringBuilder BuildOptions(StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            var options = _app.GetOptions().Where(o => o.ShowInHelpText).ToList();
            if (options.Any())
            {
                usageBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine("Options:");
                var maxOptLen = options.Max(o => o.Template.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxOptLen + 2);
                foreach (var opt in options)
                {
                    optionsBuilder.AppendFormat(outputFormat, opt.Template, opt.Description);
                    optionsBuilder.AppendLine();
                }
            }

            return optionsBuilder;
        }

        private StringBuilder BuildArguments(StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = _app.Arguments.Where(a => a.ShowInHelpText).ToList();
            if (arguments.Any())
            {
                usageBuilder.Append(" [arguments]");

                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine("Arguments:");
                var maxArgLen = arguments.Max(a => a.Name.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxArgLen + 2);
                foreach (var arg in arguments)
                {
                    argumentsBuilder.AppendFormat(outputFormat, arg.Name, arg.Description);
                    argumentsBuilder.AppendLine();
                }
            }

            return argumentsBuilder;
        }

        private StringBuilder BuildUsage()
        {
            var usageBuilder = new StringBuilder("Usage: ");
            usageBuilder.Append(_app.GetFullCommandName());
            return usageBuilder;
        }

        private StringBuilder BuildTitle()
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(_app.Description))
                titleBuilder.AppendLine(_app.Description + "\n");
            return titleBuilder;
        }
    }
}