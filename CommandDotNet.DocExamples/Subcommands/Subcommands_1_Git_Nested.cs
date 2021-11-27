using CommandDotNet.NameCasing;

namespace CommandDotNet.DocExamples.Subcommands
{
    public class Subcommands_1_Git_Nested
    {
        [Command(Description = "Fake git application")]
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.KebabCase);

            // Properties decorated with the [Subcommand] attribute will be subcommands of the host
            [Subcommand]
            public Stash Stash { get; set; }

            [Command(Description = "Commits all staged changes")]
            public void Commit(IConsole console, [Option('m')] string? commitMessage)
            {
                console.WriteLine("Commit successful");
            }
        }

        [Command(Description = "Stashes all changes when executed without any arguments")]
        [Subcommand]
        public class Stash
        {
            [DefaultCommand]
            public void StashImpl(IConsole console)
            {
                console.WriteLine("changes stashed");
            }

            [Command(Description = "Applies last stashed changes")]
            public void Pop(IConsole console)
            {
                console.WriteLine("stash popped");
            }

            [Command(Description = "Lists all stashed changes")]
            public void List(IConsole console)
            {
                console.WriteLine("here's the list of stash");
            }
        }
    }
}