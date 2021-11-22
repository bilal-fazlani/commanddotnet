namespace CommandDotNet.DocExamples.GettingStarted
{
    public class GettingStarted_OtherFeatures
    {
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner =>
                // begin-snippet: getting_started_other_features
                new AppRunner<Program>()
                    .UseDefaultMiddleware();
            // end-snippet

            public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);

            public void Subtract(IConsole console, int x, int y) => console.WriteLine(x - y);
        }


        public static BashSnippet Help = new("getting_started_other_features_help",
            Program.AppRunner,
            "dotnet calculator.dll", "--help", 0,
            @"Usage: {0} [command] [options]

Options:

  -v | --version
  Show version information

Commands:

  Add
  Subtract

Use ""{0} [command] --help"" for more information about a command.");
    }
}