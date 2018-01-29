using System.Linq;
using System.Text;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    public class BasicHelpTextProvider : IHelpProvider
    {
        private readonly AppSettings _appSettings;

        public BasicHelpTextProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        
        public string GetHelpText(ICommand app)
        {            
            var titleBuilder = BuildTitle(app);
            
            var usageBuilder = BuildUsage(app);

            var argumentsBuilder = BuildArguments(app, usageBuilder);

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
                commandsBuilder.AppendLine("Commands:");
                var maxCmdLen = commands.Max(c => c.Name.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxCmdLen + 2);
                               
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
            var options = app.GetOptions().Where(o => o.ShowInHelpText).ToList();
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

        private StringBuilder BuildArguments(ICommand app, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = app.Arguments.Where(a => a.ShowInHelpText).ToList();
                        
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
                titleBuilder.AppendLine(app.Description + "\n");
            return titleBuilder;
        }
    }
}