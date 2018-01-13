using System;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class NestedCommandsTest : TestBase
    {
        public NestedCommandsTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        //nested types
        [InlineData(3, new[]{"stash","pop"})]
        [InlineData(4, new[]{"stash","list"})]
        [InlineData(5, new[]{"stash", "--additionalFactor", "1", "list"})]
        
        // default command in nested type
        [InlineData(2, new[]{"stash"})]
        
        // typical scenario
        [InlineData(5, new[]{"commit", "-m", "added new feature"})]
        
        // external module
        [InlineData(6, new[]{"submodule","add"})]
        [InlineData(7, new[]{"submodule","remove"})]
        
        // multi level nesting
        [InlineData(8, new[]{"remote","origin", "show"})] 
        public void CanExecuteSubModules(int expectedExitCode, string[] args)
        {
            AppRunner<GitApplication> appRunner = new AppRunner<GitApplication>();
            appRunner.Run(args).Should().Be(expectedExitCode);
        }
    }

    public class GitApplication
    {
        [SubCommand]
        private class Remote
        {            
            [SubCommand]
            private class Origin
            {   
                public int Show()
                {
                    Console.WriteLine("http://bla bla");
                    return 8;
                }
            }
        }
        
        [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments\n" +
                                           "see Stash --help for further information",
            Name = "stash")]
        [SubCommand]
        private class Stash
        {
            private readonly int _additionalFactor;

            public Stash(int additionalFactor)
            {
                _additionalFactor = additionalFactor;
            }
            
            [DefaultMethod]
            public int DoStash()
            {
                Console.WriteLine($"changes stashed");
                return _additionalFactor + 2;
            }
        
            [ApplicationMetadata(Name = "pop", Description = "Applies last stashed changes")]
            public int Pop()
            {
                Console.WriteLine($"stash popped");
                return _additionalFactor + 3;
            }

            [ApplicationMetadata(Name = "list", Description = "Lists all saved stashed changes")]
            public int List()
            {
                Console.WriteLine($"here's the list of stash");
                return _additionalFactor + 4;
            }
        }
        
        [ApplicationMetadata(Name = "commit", Description = "Commits all staged changes")]
        public int Commit([Option(ShortName = "m")]string message)
        {
            Console.WriteLine($"Commit successful : {message}");
            return 5;
        }
        
        [SubCommand]
        public Submodule Submodule { get; set; }
    }
    
    public class Submodule
    {
        public int Add(string url)
        {
            Console.WriteLine($"Submodule {url} added");
            return 6;
        }

        public int Remove(string path)
        {
            Console.WriteLine($"Submodule {path} removed");
            return 7;
        }
    }
}