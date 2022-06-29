using System;
using CommandDotNet.NameCasing;
using CommandDotNet.TestTools;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands.Subcommands
{
#pragma warning disable CS8618
    public class Subcommands_Git_Composed
    {
        // begin-snippet: subcommands_git_composed
        [Command(Description = "Fake git application")]
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner =>
                new AppRunner<Program>().UseNameCasing(Case.KebabCase);

            // Properties decorated with the [Subcommand] attribute will be subcommands of the host
            [Subcommand]
            public Stash Stash { get; set; }

            [Command(Description = "Commits all staged changes")]
            public void Commit(IConsole console, [Option('m', null)] string? commitMessage)
            {
                console.WriteLine("Commit successful");
            }
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
        // end-snippet

        public static BashSnippet Help = new("subcommands_git_composed_help",
            Program.AppRunner, "git.exe", "-h", 0, @"Fake git application

Usage: git.exe [command]

Commands:

  commit  Commits all staged changes
  stash   Stashes all changes when executed without any arguments

Use ""git.exe [command] --help"" for more information about a command.");

        public static BashSnippet Commit = new("subcommands_git_composed_commit",
            Program.AppRunner.InterceptSystemConsoleWrites(), 
            "git.exe", "commit -m \"some refactoring\"", 0, "Commit successful");

        public static BashSnippet StashSnippet = new("subcommands_git_composed_stash",
            Program.AppRunner.InterceptSystemConsoleWrites(), 
            "git.exe", "stash", 0, "changes stashed");

        public static BashSnippet Pop = new("subcommands_git_composed_stash_pop",
            Program.AppRunner.InterceptSystemConsoleWrites(), 
            "git.exe", "stash pop", 0, "stash popped");

        public static BashSnippet StashHelp = new("subcommands_git_composed_stash_help",
            Program.AppRunner.InterceptSystemConsoleWrites(), 
            "git.exe", "stash -h", 0, @"Stashes all changes when executed without any arguments

Usage: git.exe stash [command]

Commands:

  list  Lists all stashed changes
  pop   Applies last stashed changes

Use ""git.exe stash [command] --help"" for more information about a command.");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}