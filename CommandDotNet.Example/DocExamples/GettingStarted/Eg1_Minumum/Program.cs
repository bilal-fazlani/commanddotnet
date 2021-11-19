using System;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg1_Minumum
{
    // begin-snippet: getting_started_calculator
    public class Program
    {
        static int Main(string[] args) => 
            new AppRunner<Program>().Run(args);

        public void Add(int x, int y) => 
            Console.WriteLine(x + y);

        public void Subtract(int x, int y) => 
            Console.WriteLine(x + y);
    }
    // end-snippet
}