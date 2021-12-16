using System;
using CommandDotNet.NameCasing;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands.Subcommands
{
    public class Subcommands_Git_Nested
    {
        // begin-snippet: subcommands_git_nested
        [Command(Description = "Fake git application")]
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.KebabCase);

            [Command(Description = "Commits all staged changes")]
            public void Commit(IConsole console, [Option('m')] string? commitMessage)
            {
                console.WriteLine("Commit successful");
            }

            [Command(Description = "Stashes all changes when executed without any arguments")]
            [Subcommand]
            public class Stash
            {
                [DefaultCommand]
                public void StashImpl(IConsole console)
                {
                    console.WriteLine("changes stashed");
                }

                [Command(Description = "Applies last stashed changes")]
                public void Pop(IConsole console)
                {
                    console.WriteLine("stash popped");
                }

                [Command(Description = "Lists all stashed changes")]
                public void List(IConsole console)
                {
                    console.WriteLine("here's the list of stash");
                }
            }
        }
        // end-snippet

        public static BashSnippet Help = new("subcommands_git_nested_help",
            Program.AppRunner, "git.exe", "-h", 0, @"Fake git application

Usage: git.exe [command]

Commands:

  commit  Commits all staged changes
  stash   Stashes all changes when executed without any arguments

Use ""git.exe [command] --help"" for more information about a command.");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}