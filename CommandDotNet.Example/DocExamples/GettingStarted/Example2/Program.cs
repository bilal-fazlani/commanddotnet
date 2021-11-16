using System;
using CommandDotNet.Rendering;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Example2
{
    // begin-snippet: getting_started_calculator_with_descriptions
    [Command(Description = "Performs mathematical calculations")]
    public class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<Program>().Run(args);
        }

        [Command(Description = "Adds two numbers")]
        public void Add(IConsole console, int value1, int value2)
        {
            console.WriteLine(value1 + value2);
        }

        [Command(Description = "Subtracts two numbers")]
        public void Subtract(IConsole console, int value1, int value2)
        {
            console.WriteLine(value1 - value2);
        }
    }
    // end-snippet
}