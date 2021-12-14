using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class Getting_Started_100_Calculator
    {
        // begin-snippet: getting-started-100-calculator
        public class Program
        {
            // this is the entry point of your application
            static int Main(string[] args)
            {
                // AppRunner<T> where T is the class defining your commands
                // You can use Program or create commands in another class
                return new AppRunner<Program>().Run(args);
            }

            // Add command with two positional arguments
            public void Add(int x, int y) => Console.WriteLine(x + y);

            // Subtract command with two positional arguments
            public void Subtract(int x, int y) => Console.WriteLine(x - y);
        }
        // end-snippet

        public static BashSnippet Help = new("getting-started-100-calculator-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Usage: {0} [command]

Commands:

  Add
  Subtract

Use ""{0} [command] --help"" for more information about a command.");

        public static BashSnippet Help_Add = new("getting-started-100-calculator-add-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Add -h", 0,
            @"Usage: {0} Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>");

        public static BashSnippet Add = new("getting-started-100-calculator-add",
            new AppRunner<Program>().InterceptSystemConsoleWrites(),
            "dotnet calculator.dll", "Add 40 20", 0,
            @"60");


        public static BashSnippet Add_Invalid = new("getting-started-100-calculator-add-invalid",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Add a 20", 2,
            @"'a' is not a valid Number");

        // Test commands not testing via BashSnippet
        [Test]
        public void Subtract_works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Subtract 40 20" },
                    Then = { Output = "20" }
                });
    }
}