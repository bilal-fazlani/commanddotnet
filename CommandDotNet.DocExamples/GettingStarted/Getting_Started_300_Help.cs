using System;
using CommandDotNet.Help;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class Getting_Started_300_Help
    {
        // begin-snippet: getting-started-300-calculator
        [Command(
            Description = "Performs mathematical calculations",
            ExtendedHelpTextLines = new []
            {
                "Include multiple lines of text",
                "Extended help of the root command is a good place to describe directives for the app"
            })]
        public class Program
        {
            static int Main(string[] args) =>
                new AppRunner<Program>().Run(args);

            [Command(
                Description = "Adds two numbers",
                UsageLines = new []
                {
                    "Add 1 2",
                    "%AppName% %CmdPath% 1 2"
                },
                ExtendedHelpText = "single line of extended help here")]
            public void Add(
                [Operand(Description = "first value")] int x,
                [Operand(Description = "second value")] int y) => Console.WriteLine(x + y);

            [Command(Description = "Subtracts two numbers")]
            public void Subtract(int x, int y) => Console.WriteLine(x - y);
        }
        // end-snippet
     
        public static BashSnippet Help = new("getting-started-300-calculator-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Performs mathematical calculations

Usage: {0} [command]

Commands:

  Add       Adds two numbers
  Subtract  Subtracts two numbers

Use ""{0} [command] --help"" for more information about a command.

Include multiple lines of text
Extended help of the root command is a good place to describe directives for the app");

        public static BashSnippet Help_Add = new("getting-started-300-calculator-add-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Add -h", 0,
            @"Adds two numbers

Usage: Add 1 2
{0} Add 1 2

Arguments:

  x  <NUMBER>
  first value

  y  <NUMBER>
  second value

single line of extended help here");

        public static BashSnippet Basic_Help_Add = new("getting-started-300-calculator-add-basic-help",
            new AppRunner<Program>(new AppSettings{Help = {TextStyle = HelpTextStyle.Basic}}),
            "dotnet calculator.dll", "Add -h", 0,
            @"Adds two numbers

Usage: Add 1 2
{0} Add 1 2

Arguments:
  x  first value
  y  second value

single line of extended help here");

        [Test]
        public void Given2Numbers_Should_Subtract() =>
            new AppRunner<Getting_Started_100_Calculator.Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Subtract 40 20" },
                    Then = { Output = "20" }
                });
    }
}