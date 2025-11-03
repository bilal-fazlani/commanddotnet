using System;
using CommandDotNet.Completions;
using FluentAssertions;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Completions;

[TestFixture]
public class Shell_Completions
{
    // begin-snippet: shell_completions_quick_start
    public class MyApp
    {
        [Subcommand]
        public CompletionCommand Completion { get; set; } = new();
        
        // your other commands...
        public void Deploy(string environment) { }
    }

    class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<MyApp>()
                .UseDefaultMiddleware()  // includes [suggest] directive
                .Run(args);
        }
    }
    // end-snippet

    // begin-snippet: shell_completions_custom_app_name
    class ProgramWithCustomName
    {
        static int Main(string[] args)
        {
            return new AppRunner<MyApp>(new AppSettings 
            { 
                Execution = { UsageAppName = "my-custom-name" }
            })
            .UseDefaultMiddleware()
            .Run(args);
        }
    }
    // end-snippet

    // begin-snippet: shell_completions_conditional
    public class MyAppWithConditionalCompletion
    {
        [Subcommand]
        public CompletionCommand? Completion { get; set; } 
#if INCLUDE_COMPLETIONS
            = new();
#endif
        
        // your commands...
        public void Deploy(string environment) { }
    }
    // end-snippet

    // begin-snippet: shell_completions_calculator
    public class Calculator
    {
        [Subcommand]
        public CompletionCommand Completion { get; set; } = new();

        public void Add(int x, int y) => Console.WriteLine(x + y);
        public void Subtract(int x, int y) => Console.WriteLine(x - y);
    }

    class CalculatorProgram
    {
        static int Main(string[] args) =>
            new AppRunner<Calculator>()
                .UseDefaultMiddleware()
                .Run(args);
    }
    // end-snippet

    // begin-snippet: shell_completions_with_enums
    public enum Environment { Dev, Staging, Production }

    public class DeployApp
    {
        [Subcommand]
        public CompletionCommand Completion { get; set; } = new();

        public void Deploy(
            Environment environment,
            [Option] bool dryRun = false)
        {
            // deploy logic
        }
    }
    // end-snippet

    [Test] public void Obligatory_test_since_snippets_cover_all_cases() => true.Should().BeTrue();
}
