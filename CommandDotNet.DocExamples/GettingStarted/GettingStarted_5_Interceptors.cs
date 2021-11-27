using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class GettingStarted_5_Interceptors
    {
        // begin-snippet: getting-started-4-interceptors
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner => new AppRunner<Calculator>();
        }

        public class Calculator
        {
            private readonly IConsole _console;

            public Calculator(IConsole console)
            {
                _console = console;
            }

            public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext ctx)
            {
                // access to AppConfig and AppSettings
                var settings = ctx.AppConfig.AppSettings;
                // access to parse results, including remaining and separated arguments 
                var parseResult = ctx.ParseResult;
                // access to command method and object, and all parent interceptor method and objects
                var pipeline = ctx.InvocationPipeline;

                // pre-execution logic here

                return next(); // Add and Subtract methods are executed within this delegate

                // post-execution logic here
            }

            public void Add(int x, int y) => _console.WriteLine(x + y);

            public void Subtract(int x, int y) => _console.WriteLine(x - y);
        }
        // end-snippet

        [TestFixture]
        public class AddCommandTests
        {
            // begin-snippet: getting-started-calculator-add-command-tests
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
            // begin-snippet: getting-started-calculator-add-command-tests-bdd
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