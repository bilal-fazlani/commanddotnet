using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using NUnit.Framework;
using Program=CommandDotNet.Example.DocExamples.GettingStarted.Example2.Program;

namespace CommandDotNet.Example.Tests.DocExamples.GettingStarted
{
    public class AddCommandTests
    {
        // begin-snippet: getting_started_calculator_add_command_tests
        [Test]
        public void Given2Numbers_Should_OutputSum()
        {
            // lala
            var result = new AppRunner<Program>()
                .RunInMem("Add 40 20");
            result.ExitCode.Should().Be(0);
            result.Console.OutText().Should().Be("60" + Environment.NewLine);
        }
        // end-snippet

        // begin-snippet: getting_started_calculator_add_command_tests_bdd
        [Test]
        public void Given2Numbers_Should_OutputSum_BDD()
        {
            new AppRunner<Program>().Verify(new Scenario
            {
                When = { Args = "Add 40 20" },
                Then = { Output = "60" + Environment.NewLine }
            });
        }
        // end-snippet
    }
}