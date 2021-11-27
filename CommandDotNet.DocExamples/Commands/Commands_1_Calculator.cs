using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands
{
    [TestFixture]
    public class Commands_1_Calculator
    {
        public class Program
        {
            // begin-snippet: commands-1-calculator
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

        public static BashSnippet Help = new("commands-1-calculator-sum-help",
            new AppRunner<Program>(), "dotnet calculator.dll", "Sum --help", 0,
            @"sums all the numbers provided

Usage: sum <int> [<int> ...]

Arguments:

  numbers (Multiple)  <NUMBER>

more details and examples could be provided here");
    }
}