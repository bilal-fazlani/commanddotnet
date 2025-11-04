# IEnvironment

`IEnvironment` is an abstraction over `System.Environment` that makes your commands testable and allows you to mock environment behavior in tests.

## Why use IEnvironment?

When your commands directly use `System.Environment`, they're tightly coupled to the actual system environment, making them difficult to test reliably. `IEnvironment` solves this by providing:

1. **Testability**: Mock environment variables and system info in tests
2. **Consistency**: Same interface whether running in production or tests
3. **Isolation**: Tests don't affect or depend on the actual system environment

## Available Members

`IEnvironment` provides access to commonly used members of `System.Environment`:

```c#
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

```c#
public class DeployCommand
{
    public void Deploy(
        IEnvironment environment,
        IConsole console,
        string target)
    {
        var env = environment.GetEnvironmentVariable("DEPLOY_ENV") ?? "dev";
        console.WriteLine($"Deploying {target} to {env} environment");
        console.WriteLine($"User: {environment.UserName}");
        console.WriteLine($"Machine: {environment.MachineName}");
    }
}
```

## Testing with TestEnvironment

The `CommandDotNet.TestTools` package provides `TestEnvironment` for use in tests:

```c#
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

```c#
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
