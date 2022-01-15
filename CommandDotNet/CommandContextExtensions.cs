namespace CommandDotNet
{
    public static class CommandContextExtensions
    {
        /// <summary>
        /// Prints help for a provided command, else the target command if parsed, else the root command.
        /// </summary>
        public static void PrintHelp(this CommandContext commandContext, Command? command = null)
        {
            command ??= commandContext.ParseResult?.TargetCommand ?? commandContext.RootCommand!;
            var helpText = commandContext.AppConfig.HelpProvider.GetHelpText(command);
            commandContext.Console.Out.WriteLine(helpText);
        }
    }
}