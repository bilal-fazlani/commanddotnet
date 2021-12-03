# RunInMem

The `RunInMem` extension method takes a fully configured AppRunner instance and runs it in memory.

```c#

[Test]
public void Checkout_NewBranch_WithoutBranchFlag_Fails()
{
    var result = new AppRunner<Git>()
        .UseDefaultMiddleware()
        .RunInMem("checkout lala");

    result.ExitCode.Should().Be(1);
    result.Console.Error.Should().Be("error: pathspec 'lala' did not match any file(s) known to git" );
}
```

## What it does

### Configure CommandDotNet logging

if `TestConfig.PrintCommandDotNetLogs = true`

### Configure TestConsole

A new `TestConsole` will be configured as the `IConsole` for the app to capture all output.

If these `RunInMem` parameters are provided, they are  

* `Func<TestConsole, string> onReadLine`
* `IEnumerable<string> pipedInput`
* `IPromptResponder promptResponder`

### Execute appRunner.Run(args)

If an argument string was provided instead of an argument array, the string is split into an array.

Executes the `AppRunner.Run` method with the argument array.

### Log Output

Log output based on `TestConfig.OnSuccess.Print` or `TestConfig.OnError.Print` settings

### Return AppRunnerResult

```c#

public class AppRunnerResult
{
    internal AppRunner Runner { get; }
    internal TestConfig Config { get; }

    public int ExitCode { get; }

    public TestConsole Console { get; }

    /// <summary>The <see cref="CommandContext"/> used during the run</summary>
    public CommandContext CommandContext { get; }

    /// <summary>The exception that escaped from <see cref="AppRunner.Run"/><br/></summary>
    public Exception? EscapedException { get; }
}

```
