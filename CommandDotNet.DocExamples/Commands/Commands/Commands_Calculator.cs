using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Commands.Commands
{
    [TestFixture]
    public class Commands_Calculator
    {
        public class Program
        {
            // begin-snippet: commands_calculator
            [Command("Sum",
                Usage = "sum <int> [<int> ...]",
                Description = "sums all the numbers provided",
                ExtendedHelpText = "more details and examples could be provided here")]
            public void Add(IConsole console, IEnumerable<int> numbers) =>
                console.WriteLine(numbers.Sum());
            // end-snippet

            public void Subtract(IConsole console, int x, int y) =>
                console.WriteLine(x + y);
        }

        public static BashSnippet Help = new("commands_calculator_sum_help",
            new AppRunner<Program>(), "dotnet calculator.dll", "Sum --help", 0,
            @"sums all the numbers provided

Usage: sum <int> [<int> ...]

Arguments:

  numbers (Multiple)  <NUMBER>

more details and examples could be provided here");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}