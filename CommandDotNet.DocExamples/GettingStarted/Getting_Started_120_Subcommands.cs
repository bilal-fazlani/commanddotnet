using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
#pragma warning disable CS8618
    [TestFixture]
    public class Getting_Started_120_Subcommands
    {
        // begin-snippet: getting-started-120-subcommands
        public class Program
        {
            static int Main(string[] args)
            {
                return new AppRunner<Program>().Run(args);
            }

            public void Add(int x, int y) => Console.WriteLine(x + y);

            public void Subtract(int x, int y) => Console.WriteLine(x - y);
            
            [Subcommand]
            // The Command attribute is NOT required.
            // The command name will default from the class name.
            // Use the Command attribute to change the name of the command,
            //   eg from Trigonometry to Trig
            [Command("Trig")]
            public class Trigonometry
            {
                public void Sine(int x) => Console.WriteLine(Math.Sin(x));

                public void Cosine(int x) => Console.WriteLine(Math.Cos(x));
            }

            // The command name will default form the class name.
            // Use RenameAs to change the name of the command.
            //   eg. from Algorithms to Algo
            [Subcommand(RenameAs = "Algo")]
            public Algorithms Algorithms { get; set; }
        }
        
        public class Algorithms
        {
            public void Fibonacci(int depth = 10)
            {
                int a = 0, b = 1, c;
                Console.WriteLine(a);
                for (int i = 2; i < depth; i++)
                {
                    c = a + b;
                    Console.WriteLine(c);
                    a = b;
                    b = c;
                }
            }
        }
        // end-snippet

        public static BashSnippet Help = new("getting-started-120-subcommands-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Usage: {0} [command]

Commands:

  Add
  Algo
  Subtract
  Trig

Use ""dotnet calculator.dll [command] --help"" for more information about a command.");

        public static BashSnippet TrigHelp = new("getting-started-120-subcommands-trig-help",
            new AppRunner<Program>(),
            "dotnet calculator.dll", "Trig --help", 0,
            @"Usage: {0} Trig [command]

Commands:

  Cosine
  Sine

Use ""{0} Trig [command] --help"" for more information about a command.");

        [Test]
        public void Add_works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Add 40 20" },
                    Then = { Output = "60" }
                });

        [Test]
        public void Subtract_works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Subtract 40 20" },
                    Then = { Output = "20" }
                });

        [Test]
        public void Sine_Works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Trig Sine 40" },
                    Then = { Output = "0.7451131604793488" }
                });

        [Test]
        public void Cosine_Works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Trig Cosine 40" },
                    Then = { Output = "-0.6669380616522619" }
                });
        
        [Test]
        public void Fibonacci_Works() =>
            new AppRunner<Program>()
                .InterceptSystemConsoleWrites()
                .Verify(new Scenario
                {
                    When = { Args = "Algo Fibonacci" },
                    Then = { Output = @"0
1
2
3
5
8
13
21
34" }
                });
    }
}