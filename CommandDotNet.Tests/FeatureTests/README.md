# Testing with scenarios.

The scenarios are modelled after the BDD Given-When-Then syntax.  
They utilize C#'s [object initializer](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) syntax to make defining a scenario as terse as possible while remaining readable.

Let's look at an example to test flag clubbing.

## Creating the test class

First, let's create the test class.

``` c#
public class FlagClubbing
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FlagClubbing(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestName()
    {
        new FlagApp()
            .VerifyScenario(_testOutputHelper, new Scenario
            {
                WhenArgs = "Club",
                Then =
                {
                    ...
                }
            });
    }
}
```

The class  `ITestOutputHelper` to the base class.  `ITestOutputHelper` is used to write to the xUnit output stream.

## Example app

Next, let's create the `FlagApp` class containing the `club` command with a single `flag` called flag, using the short name `f`.  Nest the app within the test class to avoid polluting the test namespace with apps that could conflict.

``` c#
public class FlagApp
{
    // will be injected by the test framework
    private TestOutputs TestOutputs { get; set; }

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

`TestOutputs` is a wrapper for a dictionary keyed by typed. Only one instance of any type can be captured. Classes like ClubResult are useful for capturing the content from multiple arguments.  

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
new Scenario
{
    WhenArgs = "club -h",
    Then =
    {
        // example of #1
        Result =  @"Usage: dotnet testhost.dll club [options]

  -f",
        // ... or ...
        // example of #2
        ResultsContainsTexts = { "club", "-f" }
    }
};
```

Note: there's a little black magic with the Result comparison.  The framework can leave extra spaces at the end of lines which is difficult to account for when specifying the expectation.  To help with this, the test framework will trim the end of every line and replace all line endings with `Environment.NewLine`.  This is done for the expectation and actual result before comparing them.

## Testing execution

Execution consists of parsing commands and arguments, routing to the correct command method and mapping to the parameters.  To verify these options, we can specify expected exit codes, results and outputs captured in the command method via `TestOutputs`.  See the following examples.

``` c#
new Scenario
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
new Scenario
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
new Scenario
{
    And = {AppSettings = new AppSettings{...}},
    WhenArgs = "club -f",
    Then = {...}
};
```

The default `AppSettings` can be overridden by passing one to the `And` clause.

## Injecting Dependencies

``` c#
new Scenario
{
    And = {Dependencies = {new Service1(), new Service2()}},
    WhenArgs = "club -f",
    Then = {...}
};
```

Dependencies by passing the instances into the `And` clause.

The instances will be registered by type as singletons within the context of that scenario.

## Rider & Resharper templates

Templates are included in CommandDotNet.sln.DotSettings to make it easer to create new test fixtures.

Open Template Explorer and you'll find them in the `Solution "CommandDotNet" team-shared` layer.

## Summary

Pull requests should include test coverage for their features.

This is also a good way to post a repro for a bug.