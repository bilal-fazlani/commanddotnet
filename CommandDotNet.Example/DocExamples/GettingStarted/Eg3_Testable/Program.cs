using System;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg3_Testable
{
    // begin-snippet: getting_started_calculator_testable
    [Command(Description = "Performs mathematical calculations")]
    public class Program
    {
        static int Main(string[] args) => AppRunner.Run(args);

        public static AppRunner<Program> AppRunner => new();

        [Command(Description = "Adds two numbers")]
        public void Add(IConsole console, int x, int y) => 
            console.WriteLine(x + y);

        [Command(Description = "Subtracts two numbers")]
        public void Subtract(IConsole console, int x, int y) => 
            console.WriteLine(x - y);
    }
    // end-snippet
}