using System;
using CommandDotNet.Builders;

namespace CommandDotNet.ReadLineRepl
{
    public class ReplConfig
    {
        private Func<CommandContext, string>? _sessionInitMessage;
        private Func<CommandContext, string>? _sessionHelpMessage;

        public string? AppName { get; set; }

        public ReplOption ReplOption { get; set; } = new ReplOption();

        public Func<CommandContext, string> GetSessionInitMessage
        {
            get => _sessionInitMessage ?? BuildSessionInit ;
            set => _sessionInitMessage = value ?? throw new ArgumentNullException(nameof(value));
        }


        public Func<CommandContext, string> GetSessionHelpMessage
        {
            get => _sessionHelpMessage ?? BuildSessionHelp;
            set => _sessionHelpMessage = value ?? throw new ArgumentNullException(nameof(value));
        }

        private string BuildSessionInit(CommandContext context)
        {
            var appInfo = AppInfo.GetAppInfo();
            return @$"{AppName ?? appInfo.FileName} {appInfo.Version}
Type 'help' to see interactive options
{BuildSessionHelp(context)}";
        }

        private string BuildSessionHelp(CommandContext context)
        {
            return @"Type '-h' or '--help' for the list of commands
Type 'exit', 'quit' or 'Ctrl+C + Enter' to exit.";
        }
    }

    public class ReplOption
    {
        public string? LongName { get; set; }
        public char? ShortName { get; set; }
        public string? Description { get; set; }

        internal bool IsRequested => LongName is { } || ShortName is { };
    }
}