﻿# Working with feature tests.

The scenarios are modelled after the BDD Given-When-Then syntax.  
They utilize C#'s [object initializer](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) syntax to make defining a scenario as terse as possible while remaining readable.

Let's look at an example to test flag clubbing.

## Creating the test class

First, let's create the test class.

``` c#
public class FlagClubbing : ScenarioTestBase<FlagClubbing>
{
    public FlagClubbing(ITestOutputHelper output) : base(output)
    {
    }

    public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<FlagApp>(...) {...},
                new Given<FlagApp>(...) {...}
                // see examples of Given below
            }
}
```

The class must extend from `ScenarioTestBase` and pass `ITestOutputHelper` to the base class.  `ITestOutputHelper` is used to write to the xUnit output stream.

The class must contain a static property named `Scenarios` that returns `IEnumerable<IScenario>` (which is implemented by the `Scenarios` type)

## Example app

Next, let's create the `FlagApp` class containing the `club` command with a single `flag` called flag, using the short name `f`.  Nest the app within the test class to avoid polluting the test namespace with apps that could conflict.

``` c#
public class FlagApp
{
    [InjectProperty]
    public TestOutputs TestOutputs { get; set; }

    public int Club([Option(ShortName="f")] bool flag)
    {
        Console.Out("clubbing");
        TestOutputs.Capture(flag);
        // ... or ...
        TestOutputs.Capture(new ClubResult{ Flag = flag });
    }

    public class ClubResult
    {
        public bool Flag { get; set; }
    }
}
```
We inject the `TestOutputs` property so we can capture the arguments passed to the command to compare against expectations.  (see more below)

`TestOutputs` is a wrapper for a dictionary keyed by typed.  Only one instance of any type can be captured.  Since we only need to capture a single bool value, we can don't need the `ClubResult` class.  Such a class is very useful for more complex scenarios and for `IArgumentModel`s.  

## Testing help output

There are two ways to test the output of help.

1. Compare the entire result.
  - Pros 
    - easy to create
    - easy to update after failure by replacing expectation with output from test failure
    - helpful to see entire output in context
  - Cons
    - more likely to break with changes unrelated to the test
2. Check for values within the result
  - Pros
    - less likely to break with changes unrelated to the test
    - it's more clear what is being tested for
  - Cons
    - can result in false positives when checked values are not unique enough to the scenario

``` c#
new Given<FlagApp>("help")
{
    WhenArgs = "club -h",
    Then =
    {
        // example of #1
        Result =  @"Usage: dotnet testhost.dll club [options]

Options:

  -h | --help
  Show help information

  -f",
        // ... or ...
        // example of #2
        ResultsContainsTexts = { "club", "-f" }
    }
};
```

Note: there's a little black magic with the Result comparison.  The framework can leave extra spaces at the end of lines which is difficult to account for when specifying the expectation.  To help with this, the test framework will trim the end of every line and replace all line endings with `Environment.NewLine`.  This is done for the expecatation and actual result before comparing them.

## Testing execution

Execution consists of parsing commands and arguments, routing to the correct command method and mapping to the parameters.  To verify these options, we can specify expected exit codes, results and outputs captured in the command method via `TestOutputs`.  See the following examples.

``` c#
new Given<FlagApp>("exec")
{
    WhenArgs = "club -f",
    Then =
    {
        ResultsContainsTexts = { "clubbing" },
        AllowUnspecifiedOutputs = true,
        Outputs = { new ClubResult{ Flag = true }}
    }
};
```

A success example.  

Use `ResultsContainsTexts` to verify messages output to the console.  Results is a merge of both `Standard` and `Error` output streams.  We haven't had a need to separate them for these tests.

`AllowUnspecifiedOutputs` allows other outputs to be included.  This is useful when the command class has Constructor Options that are validated by other tests.  This is false by default.

Multiple `Outputs` can be expected.  The test framework will verify each exists and all properties match.

``` c#
new Given<FlagApp>("exec failure")
{
    WhenArgs = "club -h",
    Then =
    {
        ExitCode = 1,
        ResultsContainsTexts = { "Unrecognized option '-h'" }
    }
};
```

A failure example.  

Use `ExitCode` to expect a result other than 0.

## Testing AppSettings overrides

``` c#
new Given<FlagApp>("exec with custom appsettings")
{
    And = {AppSettings = new AppSettings{...}},
    WhenArgs = "club -f",
    Then = {...}
};
```

The default `AppSettings` can be overridden by passing one to the `And` clause.


## Summary

Most scenarios can be modelled this way.  To my knowledge, overrides applied directly to `AppRunner` are not yet handled, for example, applying a CustomHelpProvider.

Pull requests should include test coverage for their features.

This is also a good way to post a repro for a bug.  Fixing the bug will fix the test and the test can be kept to avoid regressions.