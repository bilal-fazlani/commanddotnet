using System;
using CommandDotNet.NameCasing;

namespace CommandDotNet.DocExamples.Subcommands
{
    public class Subcommands_1_Git_Composed
    {
        [Command(Description = "Fake git application")]
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.KebabCase);

            [Command(Description = "Commits all staged changes")]
            public void Commit([Option('m')] string? commitMessage)
            {
                Console.WriteLine("Commit successful");
            }

            [Command(Description = "Stashes all changes when executed without any arguments")]
            [Subcommand]
            public class Stash
            {
                [DefaultCommand]
                public void StashImpl()
                {
                    Console.WriteLine("changes stashed");
                }

                [Command(Description = "Applies last stashed changes")]
                public void Pop()
                {
                    Console.WriteLine("stash popped");
                }

                [Command(Description = "Lists all stashed changes")]
                public void List()
                {
                    Console.WriteLine("here's the list of stash");
                }
            }
        }
    }
}