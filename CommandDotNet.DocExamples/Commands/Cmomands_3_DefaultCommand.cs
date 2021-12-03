using CommandDotNet.NameCasing;

namespace CommandDotNet.DocExamples.Commands
{
    public class Cmomands_3_DefaultCommand
    {
        // begin-snippet: commands-3-default-command
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

        public static BashSnippet Stash = new("commands-3-default-command-process",
            Program.AppRunner, "dotnet myapp.dll", "", 0, "");
    }
}