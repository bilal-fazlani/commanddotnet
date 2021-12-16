using System;
using CommandDotNet.NameCasing;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands.Commands
{
    public class Commands_Git
    {
        // begin-snippet: commands_git
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

        public static BashSnippet Stash = new ("commands_2_git_stash", 
            Program.AppRunner, "git", "stash", 0, "stash");

        public static BashSnippet Pop = new("commands_2_git_pop",
            Program.AppRunner, "git", "stash pop", 0, "pop");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }

}