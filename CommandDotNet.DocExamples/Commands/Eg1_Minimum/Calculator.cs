using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands.Eg1_Minimum
{
    [TestFixture]
    public class Commands_1_Calculator
    {
        public class Program
        {
            // begin-snippet: commands_1_calculator
            [Command("Sum",
                Usage = "sum <int> [<int> ...]",
                Description = "sums all the numbers provided",
                ExtendedHelpText = "more details and examples could be provided here")]
            public void Add(IEnumerable<int> numbers) =>
                Console.WriteLine(numbers.Sum());
            // end-snippet

            public void Subtract(int x, int y) =>
                Console.WriteLine(x + y);
        }

        public static BashSnippet Help = new("commands_1_calculator_sum_help",
            new AppRunner<Program>(), "dotnet calculator.dll", "Sum --help", 0,
            @"sums all the numbers provided

Usage: sum <int> [<int> ...]

Arguments:

  numbers (Multiple)  <NUMBER>

more details and examples could be provided here");
    }
}