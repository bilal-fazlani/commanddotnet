using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class GettingStarted_1_Calculator
    {
        // begin-snippet: getting_started_calculator
        public class Program
        {
            static int Main(string[] args) =>
                new AppRunner<Program>().Run(args);

            public void Add(int x, int y) =>
                Console.WriteLine(x + y);

            public void Subtract(int x, int y) =>
                Console.WriteLine(x - y);
        }
        // end-snippet

        public static BashSnippet Help = new("getting_started_1_calculator_help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Usage: {0} [command]

Commands:

  Add
  Subtract

Use ""{0} [command] --help"" for more information about a command.");

        public static BashSnippet Help_Add = new("getting_started_1_calculator_add_help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Add -h", 0,
            @"Usage: {0} Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>");

        public static BashSnippet Add = new("getting_started_1_calculator_add",
            new AppRunner<Program>().InterceptingSystemConsoleWrites(),
            "dotnet calculator.dll", "Add 40 20", 0,
            @"60");


        public static BashSnippet Add_Invalid = new("getting_started_1_calculator_add_invalid",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Add a 20", 2,
            @"'a' is not a valid Number");

        [Test]
        public void Given2Numbers_Should_Add() =>
            new AppRunner<Program>()
                .InterceptingSystemConsoleWrites()
                .Verify(new Scenario
            {
                When = { Args = Add.Args },
                Then = { Output = Add.Output }
            });

        [Test]
        public void Given2Numbers_Should_Subtract() =>
            new AppRunner<Program>()
                .InterceptingSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Subtract 40 20" },
                    Then = { Output = "20" }
                });

        [Test]
        public void GivenANonNumber_Should_OutputValidationError() =>
            new AppRunner<Program>().Verify(new Scenario
            {
                When = { Args = Add_Invalid.Args },
                Then =
                {
                    ExitCode = 2,
                    Output = Add_Invalid.Output
                }
            });
    }
}