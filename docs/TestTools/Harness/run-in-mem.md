# RunInMem

The `RunInMem` extension method takes a fully configured AppRunner instance and runs it in memory.

<!-- snippet: testtools_runinmem_error_example -->
<a id='snippet-testtools_runinmem_error_example'></a>
```cs
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
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/TestTools/TestTools_Examples.cs#L47-L65' title='Snippet source file'>snippet source</a> | <a href='#snippet-testtools_runinmem_error_example' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

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

```cs

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
