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

        // the console output   
        public string ConsoleOut { get; }
        public string ConsoleError { get; }
        public string ConsoleOutAndError { get; }
        
        // described in the scenario readme.md from above
        public TestOutputs TestOutputs { get; }

        // helpers that normalize the help output which 
        // can contain extra whitespace at the end of lines.
        public void OutputShouldBe(string expected){...}
        public bool OutputContains(string expected){...}
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
