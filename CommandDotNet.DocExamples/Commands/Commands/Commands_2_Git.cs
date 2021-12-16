using System;
using CommandDotNet.NameCasing;

namespace CommandDotNet.DocExamples.Commands
{
    public class Commands_2_Git
    {
        // begin-snippet: commands-2-git
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner => new AppRunner<Program>().UseNameCasing(Case.KebabCase);

            [Subcommand]
            public class Stash
            {
                [DefaultCommand]
                public void StashImpl(IConsole console) => console.WriteLine("stash");

                [Command(Usage = "%AppName% %CmdPath%")]
                public void Pop(IConsole console) => console.WriteLine("pop");
            }
        }
        // end-snippet

        public static BashSnippet Stash = new ("commands-2-git-stash", 
            Program.AppRunner, "git", "stash", 0, "stash");

        public static BashSnippet Pop = new("commands-2-git-pop",
            Program.AppRunner, "git", "stash pop", 0, "pop");
    }

}