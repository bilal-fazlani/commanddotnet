using System;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg2_Descriptions
{
    // begin-snippet: getting_started_calculator_with_descriptions
    [Command(Description = "Performs mathematical calculations")]
    public class Program
    {
        static int Main(string[] args) => 
            new AppRunner<Program>().Run(args);

        [Command("Sum", Description = "Adds two numbers")]
        public void Add(int x, int y) => 
            Console.WriteLine(x + y);

        [Command(Description = "Subtracts two numbers")]
        public void Subtract(int x, int y) => 
            Console.WriteLine(x - y);
    }
    // end-snippet
}