using System.Linq;
using System.Text;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    internal class BasicHelpTextProvider : IHelpProvider
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

            var commandsBuilder = BuildCommands(app, usageBuilder);
            
            var operandsBuilder = BuildOperands(app, usageBuilder);

            var optionsBuilder = BuildOptions(app, usageBuilder);

            var extendedHelpTextBuild = BuildExtendedHelpText(app);
            
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
                var outputFormat = $"  {{0, -{maxCmdLen + 2}}}{{1}}";
                               
                foreach (var cmd in commands.OrderBy(c => c.Name))
                {
                    commandsBuilder.AppendFormat(outputFormat, cmd.Name, cmd.Description);
                    commandsBuilder.AppendLine();
                }

                commandsBuilder.AppendLine();
                
                if (app.OptionHelp != null)
                {
                    commandsBuilder.Append("Use \"");
                    commandsBuilder.AppendUsageCommandNames(app, _appSettings);
                    commandsBuilder.AppendLine($" [command] --{app.OptionHelp.LongName}\" for more information about a command.");
                }
            }

            return commandsBuilder;
        }

        private  StringBuilder BuildOptions(ICommand app, StringBuilder usageBuilder)
        {
            var optionsBuilder = new StringBuilder();
            var options = app.GetOptions()
                .Where(o => o.ShowInHelpText)
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

        private StringBuilder BuildOperands(ICommand app, StringBuilder usageBuilder)
        {
            var argumentsBuilder = new StringBuilder();

            var arguments = app.Operands.Where(a => a.ShowInHelpText).ToList();
                        
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

        private StringBuilder BuildUsage(ICommand app)
        {
            return new StringBuilder("Usage: ").AppendUsageCommandNames(app, _appSettings);
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