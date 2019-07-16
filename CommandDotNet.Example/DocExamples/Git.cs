using System;

namespace CommandDotNet.Example.DocExamples
{
    [ApplicationMetadata(Name = "git", Description = "Fake git application")]
    public class Git
    {
        [ApplicationMetadata(Description = "Commits all staged changes")]
        public void Commit([Option(ShortName = "m")]string commitMessage)
        {
            Console.WriteLine("Commit successful");
        }

        [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments")]
        [SubCommand]
        public class Stash
        {
            [DefaultMethod]
            public void StashDefaultCommand()
            {
                Console.WriteLine($"changes stashed");
            }

            [ApplicationMetadata(Description = "Applies last stashed changes")]
            public void Pop()
            {
                Console.WriteLine($"stash popped");
            }

            [ApplicationMetadata(Description = "Lists all stashed changes")]
            public void List()
            {
                Console.WriteLine($"here's the list of stash");
            }
        }
    }
}