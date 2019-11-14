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
        public string ConsoleOut { get; }
        public string ConsoleError { get; }
        public string ConsoleOutAndError { get; }
        public TestOutputs TestOutputs { get; }
        public void OutputShouldBe(string expected){...}
        public bool OutputContains(string expected){...}
        public bool OutputNotContains(string expected){...}
    }
```

`ConsoleOut` is what you'd see in the console window.

`TestOutputs` are described in the [Scenario readme.md](FeatureTests/README.md)

`OutputShouldBe` and `OutputContains` are helpers that normalize the help output which can contain extra whitespace at the end of lines.

## Commonalities
Both patterns require an app class be defined for the test.  The app(s) should be defined within the test class to avoid polluting the test namespace.

Both patterns ensure Console.Out is never used keeping the option open for parallelized tests.

Both patterns test at the public API layer which has several benefits

* The full pipeline is tested.  This prevents failures where unit tests isolation interactions with one aspect of the framework but forgot others. 
* Refactoring internals can be done without changing any tests.  
* It's easier to identify breaking changes.  If a test needs to change, it indicates a breaking change because either behavior or the API has been modified.

## Summary

Every feature should have tests following these patterns.
