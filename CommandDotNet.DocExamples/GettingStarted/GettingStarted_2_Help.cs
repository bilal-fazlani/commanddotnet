using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class GettingStarted_2_Help
    {
        // begin-snippet: getting_started_2_calculator
        [Command(Description = "Performs mathematical calculations")]
        public class Program
        {
            static int Main(string[] args) =>
                new AppRunner<Program>().Run(args);

            [Command("Sum", Description = "Adds two numbers")]
            public void Add(int x, int y) => Console.WriteLine(x + y);

            [Command(Description = "Subtracts two numbers")]
            public void Subtract(int x, int y) => Console.WriteLine(x - y);
        }
        // end-snippet
     
        public static BashSnippet Help = new("getting_started_2_calculator_help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Performs mathematical calculations

Usage: {0} [command]

Commands:

  Subtract  Subtracts two numbers
  Sum       Adds two numbers

Use ""{0} [command] --help"" for more information about a command.");

        public static BashSnippet Help_Add = new("getting_started_2_calculator_add_help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Sum -h", 0,
            @"Adds two numbers

Usage: {0} Sum <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>");
        
        [Test]
        public void Given2Numbers_Should_Subtract() =>
            new AppRunner<GettingStarted_1_Calculator.Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Subtract 40 20" },
                    Then = { Output = "20" }
                });
    }
}