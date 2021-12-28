using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    public class Getting_Started_400_Testing
    {
        public class Program
        {
            // begin-snippet: getting-started-400-calculator
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner => new AppRunner<Program>();
            // end-snippet

            // begin-snippet: getting-started-400-calculator-console
            public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
            
            public void Subtract(IConsole console, int x, int y) => console.WriteLine(x - y);
            // end-snippet
        }

        public class ConsoleInterception
        {
            // begin-snippet: getting-started-400-calculator-console-intercept
            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .InterceptSystemConsoleWrites();
            // end-snippet
        }

        [TestFixture]
        public class AddCommandTests
        {
            // begin-snippet: getting-started-400-calculator-add-command-tests
            [Test]
            public void Given2Numbers_Should_OutputSum()
            {
                var result = Program.AppRunner.RunInMem("Add 40 20");
                result.ExitCode.Should().Be(0);
                result.Console.OutText().Should().Be("60");
            }
            // end-snippet
        }

        [TestFixture]
        public class AddCommandTestsBDD
        {
            // begin-snippet: getting-started-400-calculator-add-command-tests-bdd
            [Test]
            public void Given2Numbers_Should_OutputSum() =>
                Program.AppRunner.Verify(new Scenario
                {
                    When = { Args = "Add 40 20" },
                    Then = { Output = "60" }
                });

            [Test]
            public void GivenANonNumber_Should_OutputValidationError() =>
                Program.AppRunner.Verify(new Scenario
                {
                    When = { Args = "Add a 20" },
                    Then =
                    {
                        ExitCode = 2, // validations exit code = 2
                        Output = "'a' is not a valid Number"
                    }
                });
            // end-snippet
        }
    }
}