using System;

namespace CommandDotNet.Example.Commands
{
    [Command(Description = "Fake git application to demonstrate nested sub-commands. Does NOT interact with git.")]
    public class Git
    {
        [Subcommand]
        public Submodule? SubmoduleProperty { get; set; }

        [Subcommand]
        public class Remote
        {
            [Subcommand]
            public class Origin
            {
                public void Show()
                {
                    Console.WriteLine("remote origin: master");
                }
            }
        }
        
        [Command("stash", Description = "Stashes all changes when executed without any arguments. " +
                                           "See stash --help for further information")]
        [Subcommand]
        public class Stash
        {
            [DefaultCommand]
            public void DoStash()
            {
                Console.WriteLine("changes stashed");
            }
        
            [Command("pop", Description = "Applies last stashed changes")]
            public void Pop()
            {
                Console.WriteLine("stash popped");
            }

            [Command("list", Description = "Lists all saved stashed changes")]
            public void List()
            {
                Console.WriteLine("here's the list of stash");
            }
        }
        
        [Command("commit", Description = "Commits all staged changes")]
        public void Commit([Option('m')]string commitMessage)
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