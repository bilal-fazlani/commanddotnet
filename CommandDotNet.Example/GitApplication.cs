using System;
using CommandDotNet.Attributes;

namespace CommandDotNet.Example
{
    public class GitApplication
    {
        public Submodule Submodule { get; set; }

        public class Stash
        {
            [DefaultMethod]
            public void DoStash()
            {
                Console.WriteLine($"changes stashed");
            }
        
            public void Pop()
            {
                Console.WriteLine($"stash popped");
            }

            public void List()
            {
                Console.WriteLine($"here's the list of stash");
            }
        }
        
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