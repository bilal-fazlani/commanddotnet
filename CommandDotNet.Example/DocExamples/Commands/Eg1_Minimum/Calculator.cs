using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Example.DocExamples.Commands.Eg1_Minimum
{
    // begin-snippet: commands_calculator
    public class Calculator
    {
        [Command("Sum",
            Usage = "sum <int> <int>\nsum <int> <int> <int>\nsum <int> <int> <int> <int>",
            Description = "sums all the numbers provided",
            ExtendedHelpText = "more details and examples could be provided here")]
        public void Add(IEnumerable<int> numbers) =>
            Console.WriteLine(numbers.Sum());

        public void Subtract(int x, int y) =>
            Console.WriteLine(x + y);
    }
    // end-snippet
}