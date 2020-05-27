using CommandDotNet.Example.Commands;

namespace CommandDotNet.Example
{
    [Command(
        ExtendedHelpText = "Directives:\n" +
                           "  [debug] to attach a debugger to the app\n" +
                           "  [parse] to output how the inputs were tokenized\n" +
                           "  [log] to output framework logs to the console\n" +
                           "  [log:debug|info|warn|error|fatal] to output framework logs for the given level or above\n" +
                           "\n" +
                           "directives must be specified before any commands and arguments.\n" +
                           "\n" +
                           "Example: %AppName% [debug] [parse] [log:info] cancel-me")]
    internal class Examples
    {
        [SubCommand]
        public Git Git { get; set; } = null!;

        [SubCommand]
        public Math Math { get; set; } = null!;

        [SubCommand]
        public Models Models { get; set; } = null!;

        [SubCommand]
        public Pipes Pipes { get; set; } = null!;

        [SubCommand]
        public CancelMe CancelMe { get; set; } = null!;

        [SubCommand]
        public Commands.Prompts Prompts { get; set; } = null!;
    }
}
