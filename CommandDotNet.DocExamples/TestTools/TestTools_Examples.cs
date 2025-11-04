using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.DocExamples.TestTools;

public static class TestTools_Examples
{
    // begin-snippet: testtools_runinmem_example
    public static void RunInMem_Example()
    {
        var result = new AppRunner<App>()
            .RunInMem("Add 2 3");

        // result.ExitCode.Should().Be(0);
        // result.Console.Out.Should().Contain("5");
    }

    private class App
    {
        public void Add(IConsole console, int x, int y) =>
            console.WriteLine(x + y);
    }
    // end-snippet

    // begin-snippet: testtools_bdd_verify_example
    public static void BDD_Verify_Example()
    {
        new AppRunner<App>()
            .Verify(new Scenario
            {
                When = 
                {
                    Args = "Add 2 3"
                },
                Then =
                {
                    Output = "5"
                }
            });
    }
    // end-snippet

    // begin-snippet: testtools_runinmem_error_example
    public class Git
    {
        public void Checkout(string branch)
        {
            System.Console.Error.WriteLine($"error: pathspec '{branch}' did not match any file(s) known to git");
        }
    }

    public static void RunInMem_Error_Example()
    {
        var result = new AppRunner<Git>()
            .UseDefaultMiddleware()
            .RunInMem("checkout lala");

        // result.ExitCode.Should().Be(1);
        // result.Console.Error.Should().Contain("error: pathspec 'lala' did not match any file(s) known to git");
    }
    // end-snippet
}
