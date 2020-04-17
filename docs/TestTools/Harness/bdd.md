# BDD Verify

The BDD style uses the `Verify` extension method which wraps [RunInMem](run-in-mem.md) with assertions defined in a BDD manner using [object initializers](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) for some syntactic goodness.


```c#
[Test]
public void Checkout_NewBranch_WithoutBranchFlag_Fails()
{
    var result = new AppRunner<Git>()
        .UseDefaultMiddleware()
        .Verify(new Scenario
        {
            When = { Args = "checkout lala" },
            Then =
            {
                ExitCode = 1,
                Output = "error: pathspec 'lala' did not match any file(s) known to git"
            }
        });
}
```

## Orchestration

First, it calls `RunInMem` and finally it returns an `AppRunnerResult`, so all options for `RunInMem` are available in `Verify`.

Additionally it

* Captures any exceptions, returns exit code of 1 and outputs the message to the console, as you would see in the shell.
* Asserts
    * ExitCode... asserts is 0 when ExitCode is specified
    * Console Output
    * TestCaptures

This approach works well with BDD frameworks like [SpecFlow](https://specflow.org/) where scenarios can be defined in other sources and mapped to code.

## More on BDD

Verify uses the Scenario to follow the Given-When-Then BDD syntax.

The AppRunner is the context of the APP.
