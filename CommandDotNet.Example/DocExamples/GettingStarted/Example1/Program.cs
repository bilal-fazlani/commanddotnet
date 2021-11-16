using System;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Example1
{
    // begin-snippet: getting_started_calculator
    public class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<Program>().Run(args);
        }
        
        public void Add(int value1, int value2)
        {
            Console.WriteLine(value1 + value2);
        }

        public void Subtract(int value1, int value2)
        {
            Console.WriteLine(value1 + value2);
        }
    }
    // end-snippet
}