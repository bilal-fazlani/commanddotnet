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
            CommandLineApplication target = GetTarget();
            
            var titleBuilder = BuildTitle(target);
            
            var usageBuilder = BuildUsage(target);

            var argumentsBuilder = BuildArguments(target, usageBuilder);

            var optionsBuilder = BuildOptions(target, usageBuilder);

            var commandsBuilder = BuildCommands(target, usageBuilder);

            if (target.AllowArgumentSeparator)
            {
                usageBuilder.Append(" [[--] <arg>...]");
            }

            usageBuilder.AppendLine();

            return titleBuilder
                + usageBuilder.ToString()
                + argumentsBuilder
                + optionsBuilder
                + commandsBuilder
                + target.ExtendedHelpText;
        }

        private CommandLineApplication GetTarget()
        {
            return _app;
        }

        private StringBuilder BuildCommands(CommandLineApplication target, StringBuilder usageBuilder)
        {
            var commandsBuilder = new StringBuilder();
            var commands = target.Commands.Where(c => c.ShowInHelpText).ToList();
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

                if (target.OptionHelp != null)
                {
                    commandsBuilder.AppendLine();
                    commandsBuilder.AppendFormat(
                        $"Use \"{target.GetFullCommandName()} [command] --{target.OptionHelp.LongName}\" for more information about a command.");
                    commandsBuilder.AppendLine();
                }
            }

            return commandsBuilder;
        }

        private static StringBuilder BuildOptions(CommandLineApplication target, StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            var options = target.GetOptions().Where(o => o.ShowInHelpText).ToList();
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

        private static StringBuilder BuildArguments(CommandLineApplication target, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = target.Arguments.Where(a => a.ShowInHelpText).ToList();
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

        private StringBuilder BuildUsage(CommandLineApplication target)
        {
            var usageBuilder = new StringBuilder("Usage: ");
            usageBuilder.Append(target.GetFullCommandName());
            return usageBuilder;
        }

        private StringBuilder BuildTitle(CommandLineApplication target)
        {
            var titleBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(target.Description))
                titleBuilder.AppendLine(target.Description + "\n");
            return titleBuilder;
        }
    }
}