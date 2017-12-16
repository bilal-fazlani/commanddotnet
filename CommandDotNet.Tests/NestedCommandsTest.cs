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
        [InlineData(2, new[]{"stash"})]
        [InlineData(3, new[]{"stash","pop"})]
        [InlineData(4, new[]{"stash","list"})]
        [InlineData(5, new[]{"commit", "-m", "added new feature"})]
        [InlineData(6, new[]{"submodule","add"})]
        [InlineData(7, new[]{"submodule","remove"})]
        public void CanExecuteSubModules(int expectedExitCode, string[] args)
        {
            AppRunner<GitApplication> appRunner = new AppRunner<GitApplication>();
            appRunner.Run(args).Should().Be(expectedExitCode);
        }
    }

    public class GitApplication
    {
        [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments\n" +
                                           "see Stash --help for further information",
            Name = "stash")]
        public class Stash
        {
            [DefaultMethod]
            public int DoStash()
            {
                Console.WriteLine($"changes stashed");
                return 2;
            }
        
            [ApplicationMetadata(Name = "pop", Description = "Applies last stashed changes")]
            public int Pop()
            {
                Console.WriteLine($"stash popped");
                return 3;
            }

            [ApplicationMetadata(Name = "list", Description = "Lists all saved stashed changes")]
            public int List()
            {
                Console.WriteLine($"here's the list of stash");
                return 4;
            }
        }
        
        [ApplicationMetadata(Name = "commit", Description = "Commits all staged changes")]
        public int Commit([Argument(ShortName = "m", RequiredString = true)]string message)
        {
            Console.WriteLine($"Commit successful : {message}");
            return 5;
        }
        
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