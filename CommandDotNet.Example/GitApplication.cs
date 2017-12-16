using System;
using System.Runtime.InteropServices;
using CommandDotNet.Attributes;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(Description = "Fake git application", Name = "git")]
    public class GitApplication
    {
        public Submodule SubmoduleProperty { get; set; }

        public class Remote
        {
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