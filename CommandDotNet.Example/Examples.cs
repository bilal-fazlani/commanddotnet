using CommandDotNet.Example.Commands;
using Git = CommandDotNet.Example.Commands.Git;

namespace CommandDotNet.Example
{
    [Command(
        // begin-snippet: extended_help_text
        ExtendedHelpText = "Directives:\n" +
                           "  [debug] to attach a debugger to the app\n" +
                           "  [parse] to output how the inputs were tokenized\n" +
                           "  [log] to output framework logs to the console\n" +
                           "  [log:debug|info|warn|error|fatal] to output framework logs for the given level or above\n" +
                           "\n" +
                           "directives must be specified before any commands and arguments.\n" +
                           "\n" +
                           "Example: %AppName% [debug] [parse] [log:info] math")]
        // end-snippet
    internal class Examples
    {
        private static bool _inSession;

        [DefaultCommand]
        public void StartSession(
            CommandContext context,
            InteractiveSession interactiveSession, 
            [Option('i')] bool interactive)
        {
            if (interactive && !_inSession)
            {
                context.Console.WriteLine("start session");
                _inSession = true;
                interactiveSession.Start();
            }
            else
            {
                context.Console.WriteLine($"no session {interactive} {_inSession}");
                context.ShowHelpOnExit = true;
            }
        }

        [Subcommand]
        public Git Git { get; set; } = null!;

        [Subcommand]
        public Math Math { get; set; } = null!;

        [Subcommand]
        public Models Models { get; set; } = null!;

        [Subcommand]
        public Pipes Pipes { get; set; } = null!;

        [Subcommand]
        public CancelMe CancelMe { get; set; } = null!;

        [Subcommand]
        public Commands.Prompts Prompts { get; set; } = null!;
    }
}
