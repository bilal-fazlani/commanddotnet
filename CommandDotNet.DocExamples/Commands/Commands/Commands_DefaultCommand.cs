namespace CommandDotNet.DocExamples.Commands.Commands
{
    public class Commands_DefaultCommand
    {
        // begin-snippet: commands_default_command
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner => new AppRunner<Program>();

            [DefaultCommand]
            public void Process()
            {
                // do very important stuff
            }
        }
        // end-snippet

        public static BashSnippet Stash = new("commands_default_command_process",
            Program.AppRunner, "dotnet myapp.dll", "", 0, "");
    }
}