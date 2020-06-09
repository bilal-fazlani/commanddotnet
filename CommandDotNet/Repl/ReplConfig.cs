using System;
using CommandDotNet.Builders;

namespace CommandDotNet.Repl
{
    public class ReplConfig
    {
        // TEST:
        // Default ReplConfig
        // - uses ReplOptionInfo.Default
        // - Default PromptTextCallback
        // - Default SessionInitMessageCallback
        //   - uses Config.AppName ?? UsageAppName ?? AppInfo.FileName
        //   - includes AppInfo.Version
        //   - includes DefaultSessionHelp
        // - Default SessionHelpMessageCallback
        // - Default ReadLine is ctx.Console.In.ReadLine
        // - cannot set to null: ReadLine, PromptTextCallback, SessionInitMessageCallback, SessionHelpMessageCallback
        // - can set to null: ReplOptionInfo, AppName

        public string? AppName { get; set; }

        private Func<CommandContext, string?>? _readLine;
        public Func<CommandContext, string?> ReadLine
        {
            get => _readLine ?? (ctx => ctx.Console.In.ReadLine());
            set => _readLine = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Func<CommandContext, string?>? _promptTextCallback;
        public Func<CommandContext, string?> PromptTextCallback
        {
            get => _promptTextCallback ?? (ctx => ">>> ");
            set => _promptTextCallback = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Func<CommandContext, string>? _sessionInitMessage;
        public Func<CommandContext, string> SessionInitMessageCallback
        {
            get => _sessionInitMessage ?? DefaultSessionInit;
            set => _sessionInitMessage = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Func<CommandContext, string>? _sessionHelpMessage;
        public Func<CommandContext, string> SessionHelpMessageCallback
        {
            get => _sessionHelpMessage ?? DefaultSessionHelp;
            set => _sessionHelpMessage = value ?? throw new ArgumentNullException(nameof(value));
        }

        private string DefaultSessionInit(CommandContext context)
        {
            var appInfo = AppInfo.Instance;
            var appName = AppName
                          ?? context.AppConfig.AppSettings.Help.UsageAppName
                          ?? appInfo.FileName;
            return @$"{appName} {appInfo.Version}
Type 'help' to see interactive options
{DefaultSessionHelp(context)}";
        }

        private string DefaultSessionHelp(CommandContext context)
        {
            return @"Type '-h' or '--help' for the list of commands
Type 'exit', 'quit' or 'Ctrl+C then Enter' to exit.";
        }
    }
}
