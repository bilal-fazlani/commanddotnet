using System;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(Name = "git", Description = "Fake git application")]
    public class GitApplication
    {
        [SubCommand]
        public Submodule SubmoduleProperty { get; set; }

        [SubCommand]
        public class Remote
        {
            [SubCommand]
            public class Origin
            {
                public void Show()
                {
                    Console.WriteLine("remote origin: master");
                }
            }
        }
        
        [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments. " +
                                           "See stash --help for further information",
            Name = "stash")]
        [SubCommand]
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
        public void Commit([Option(ShortName = "m")]string commitMessage)
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