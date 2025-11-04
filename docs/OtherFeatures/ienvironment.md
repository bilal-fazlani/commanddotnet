# IEnvironment

`IEnvironment` is an abstraction over `System.Environment` that makes your commands testable and allows you to mock environment behavior in tests.

## Why use IEnvironment?

When your commands directly use `System.Environment`, they're tightly coupled to the actual system environment, making them difficult to test reliably. `IEnvironment` solves this by providing:

1. **Testability**: Mock environment variables and system info in tests
2. **Consistency**: Same interface whether running in production or tests
3. **Isolation**: Tests don't affect or depend on the actual system environment

## Available Members

`IEnvironment` provides access to commonly used members of `System.Environment`:

```cs
public interface IEnvironment
{
    string CommandLine { get; }
    string CurrentDirectory { get; set; }
    int ExitCode { get; set; }
    string MachineName { get; }
    string NewLine { get; }
    OperatingSystem OSVersion { get; }
    string UserName { get; }
    Version Version { get; }
    
    IDictionary GetEnvironmentVariables();
    string? GetEnvironmentVariable(string variable);
    void SetEnvironmentVariable(string variable, string? value);
}
```

## Usage in Commands

Inject `IEnvironment` as a parameter in your command methods, just like `IConsole`:

<!-- snippet: ienvironment_inject_example -->
<a id='snippet-ienvironment_inject_example'></a>
```cs
public class DeployCommand
{
    public void Deploy(
        IConsole console,
        IEnvironment environment,
        string target)
    {
        console.Out.WriteLine($"Deploying to {target}");
        console.Out.WriteLine($"Current user: {environment.UserName}");
        console.Out.WriteLine($"Machine: {environment.MachineName}");
        
        var apiKey = environment.GetEnvironmentVariable("DEPLOY_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            console.Error.WriteLine("DEPLOY_API_KEY environment variable not set");
            return;
        }
        
        // Use apiKey for deployment
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/IEnvironment_Examples.cs#L5-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-ienvironment_inject_example' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Testing with TestEnvironment

The `CommandDotNet.TestTools` package provides `TestEnvironment` for use in tests:

```cs
[Test]
public void Deploy_UsesEnvironmentVariable()
{
    var testEnv = new TestEnvironment
    {
        Variables = { ["DEPLOY_ENV"] = "production" }
    };
    
    var result = new AppRunner<DeployCommand>()
        .UseTestEnv(testEnv)
        .RunInMem("Deploy myapp");
        
    result.Console.AllText().Should().Contain("production");
}
```

## UseTestEnv Extension

The `.UseTestEnv()` extension method configures the AppRunner to use your test environment:

```cs
appRunner.UseTestEnv(testEnvironment);
```

This replaces the default `SystemEnvironment` with your `TestEnvironment` for the duration of the test.

## SystemEnvironment

`SystemEnvironment` is the default implementation that wraps `System.Environment`. It's automatically used when you don't specify a test environment.

## Best Practices

1. **Always use IEnvironment in commands**: Avoid direct `System.Environment` calls
2. **Use TestEnvironment in tests**: Makes tests reliable and fast
3. **Set only needed variables**: In tests, only set the environment variables your test actually needs
4. **Don't modify in production**: `IEnvironment` should be read-only in most commands; modifications should be deliberate

## Related

- [Testing Overview](../TestTools/overview.md) - General testing guidance
- [IConsole](iconsole.md) - Similar abstraction for console I/O
- [Default Values from EnvVar](../ArgumentValues/default-values-from-config.md#environment-variables) - Using environment variables for argument defaults
