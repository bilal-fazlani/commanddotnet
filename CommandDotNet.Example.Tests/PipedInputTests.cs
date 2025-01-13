using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace CommandDotNet.Example.Tests;

[TestFixture]
public class PipedInputTests
{
    [Test]
    public void PipedInput_Should_UnionWithUserSuppliedValues()
    {
        var result = new AppRunner<App>()
            .RunInMem("List aaa bbb", pipedInput: ["ccc", "ddd"]);

        result.ExitCode.Should().Be(0);
        result.Console.AllText().Should().Be(@"aaa
bbb
ccc
ddd");
    }

    [Test]
    public void PipedInput_Should_UnionWithUserSuppliedValues_BDD()
    {
        new AppRunner<App>()
            .Verify(new Scenario
            {
                When=
                {
                    Args = "List aaa bbb",
                    PipedInput = ["ccc", "ddd"]
                },
                Then =
                {
                    Output = @"aaa
bbb
ccc
ddd"
                }
            });
    }

    private class App
    {
        [UsedImplicitly]
        public void List(IConsole console, List<string> args) =>
            console.WriteLine(string.Join(Environment.NewLine, args));
    }
}