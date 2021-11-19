using System;

namespace CommandDotNet.Example.DocExamples
{
    [Command("git", Description = "Fake git application")]
    public class Git
    {
        [Command(Description = "Commits all staged changes")]
        public void Commit([Option('m')]string commitMessage)
        {
            Console.WriteLine("Commit successful");
        }

        [Command(Description = "Stashes all changes when executed without any arguments")]
        [Subcommand]
        public class Stash
        {
            [DefaultCommand]
            public void StashDefaultCommand()
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