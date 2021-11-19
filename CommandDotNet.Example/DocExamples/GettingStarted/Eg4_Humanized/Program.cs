using System;
using CommandDotNet.NameCasing;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg4_Humanized
{
    [Command(Description = "Performs mathematical calculations")]
    public class Program
    {
        static int Main(string[] args) => AppRunner.Run(args);

        public static AppRunner AppRunner =>
            // begin-snippet: getting_started_calculator_humanized
            new AppRunner<Program>()
                .UseDefaultMiddleware()
                .UseNameCasing(Case.LowerCase);
            // end-snippet

        [Command(Description = "Adds two numbers")]
        public void Add(IConsole console, int x, int y) => 
            console.WriteLine(x + y);

        [Command(Description = "Subtracts two numbers")]
        public void Subtract(IConsole console, int x, int y) => 
            console.WriteLine(x - y);
    }
}