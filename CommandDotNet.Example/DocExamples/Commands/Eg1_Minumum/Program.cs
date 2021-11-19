using System;

namespace CommandDotNet.Example.DocExamples.Commands.Eg1_Minumum
{
    // begin-snippet: commands_calculator
    public class Class
    {
        [Command("sum",
            Usage = "sum <int> <int>",
            Description = "sums two numbers",
            ExtendedHelpText = "more details and examples")]
        public void Add(int x, int y) =>
            Console.WriteLine(x + y);

        public void Subtract(int x, int y) =>
            Console.WriteLine(x + y);
    }
    // end-snippet
}