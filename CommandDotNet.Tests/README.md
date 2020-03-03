# Testing in CommandDotNet

When implementing new features, there are two patterns for writing feature tests.

## BDD Scenario pattern
This [readme.md](FeatureTests/README.md) will walk you through how to use it.  This pattern gives a succint, declarative way to specify the scenario from the perspective of calling the app from the command line.

## AppRunner extensions
The second pattern is run an instance of the AppRunner using `AppRunner.RunInMem(...)`

`RunInMem` returns an instance of [AppRunnerResult](../CommandDotNet.TestTools/AppRunnerResult.cs)

``` c#
    public class AppRunnerResult
    {
        public int ExitCode { get; }

        /// <summary>
        /// The combination of <see cref="Console.Error"/> and <see cref="Console.Out"/>
        /// in the order they were written from the app.<br/>
        /// This is how the output would appear in the shell.
        /// </summary>
        public string ConsoleOutAndError { get; }

        /// <summary>The error output only</summary>
        public string ConsoleOut { get; }

        /// <summary>The standard output only</summary>
        public string ConsoleError { get; }
        
        // described in the scenario readme.md from above
        /// <summary>
        /// <see cref="TestOutputs"/> captured in the command class.
        /// The command class must have a public <see cref="TestOutputs"/> property for this to work.<br/>
        /// This is a convenience for testing how inputs are mapped into the command method parameters.<br/>
        /// Useful for testing middleware components, not the business logic of your commands.
        /// </summary>
        public TestOutputs TestOutputs { get; }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public void OutputShouldBe(string expected){...}

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public bool OutputContains(string expected){...}

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public bool OutputNotContains(string expected){...}
    }
```

## Commonalities
Both patterns require an app class be defined for the test.  The app(s) should be defined within the test class to avoid polluting the test namespace.

Both patterns ensure Console.Out is never used keeping the option open for parallelized tests.

Both patterns test at the public API layer which has several benefits

* The full pipeline is tested. Integration between components cannot be accidently skipped.  It is harder to introduce bugs due to not understanding the interactions between subsystems.
* Refactoring internals can be done without changing any tests.
* It's easier to identify breaking changes. If a test needs to change, it indicates a breaking change as either behavior or the API has been modified.

## Summary

Every feature should have tests following these patterns.
