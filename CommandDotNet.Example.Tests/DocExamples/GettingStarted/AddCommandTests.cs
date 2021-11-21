using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using NUnit.Framework;
using Program=CommandDotNet.Example.DocExamples.GettingStarted.Eg3_Testable.Program;

namespace CommandDotNet.Example.Tests.DocExamples.GettingStarted
{
    public class AddCommandTests
    {
        // begin-snippet: getting_started_calculator_add_command_tests
        [Test]
        public void Given2Numbers_Should_OutputSum()
        {
            // lala
            var result = Program.AppRunner.RunInMem("Add 40 20");
            result.ExitCode.Should().Be(0);
            result.Console.OutText().Should().Be("60");
        }
        // end-snippet
    }

    public class AddCommandTestsBDD
    {
        // begin-snippet: getting_started_calculator_add_command_tests_bdd
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
                    ExitCode = 2,
                    Output = "'a' is not a valid Number"
                }
            });
        // end-snippet
    }
}