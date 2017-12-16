using System;
using CommandDotNet.Attributes;

namespace CommandDotNet.Example
{
    public class GitApplication
    {
        public Submodule Submodule { get; set; }

        [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments\n" +
                                           "see Stash --help for further information",
            Name = "stash")]
        public class Stash
        {
            [DefaultMethod]
            public void DoStash()
            {
                Console.WriteLine($"changes stashed");
            }
        
            [ApplicationMetadata(Name = "pop", Description = "Applies last stashed changes")]
            public void Pop()
            {
                Console.WriteLine($"stash popped");
            }

            [ApplicationMetadata(Name = "list", Description = "Lists all saved stashed changes")]
            public void List()
            {
                Console.WriteLine($"here's the list of stash");
            }
        }
        
        [ApplicationMetadata(Name = "commit", Description = "Commits all staged changes")]
        public void Commit(string commitMessage)
        {
            Console.WriteLine("Commit successful");
        }
    }
    
    public class Submodule
    {
        public void Add(string name)
        {
            Console.WriteLine($"submodule added: {name}");
        }

        public void Remove(string name)
        {
            Console.WriteLine($"submodule: {name} has been removed");
        }
    }
}