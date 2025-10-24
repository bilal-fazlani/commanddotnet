# Description Method

The `DescriptionMethodAttribute` allows you to generate argument descriptions dynamically at runtime, rather than using static text. This is useful when:

* Descriptions depend on configuration or environment variables
* You want to display available options discovered at runtime
* Descriptions need to show current state or timestamps
* You want to avoid duplicating lists in both code and documentation

## Basic Usage

Apply the `[DescriptionMethod]` attribute to an option or operand parameter and specify the name of a static method that returns a string:

<!-- snippet: description_method_example -->
<a id='snippet-description_method_example'></a>
```cs
[Command(Description = "Deploy services with dynamic target discovery")]
public void Deploy(
    [Option('t', "targets")]
    [DescriptionMethod(nameof(GetAvailableTargets))]
    string[] targets,
    
    [Option('e', "environment")]
    [DescriptionMethod(nameof(GetAvailableEnvironments))]
    string environment = "dev")
{
    Console.WriteLine($"Deploying targets: {string.Join(", ", targets)} to {environment}");
}

private static string GetAvailableTargets()
{
    // Dynamic discovery of available targets
    var availableTargets = new[] { "app", "database", "cache", "notifications" };
    return $"Available targets: {string.Join(", ", availableTargets)}";
}

private static string GetAvailableEnvironments()
{
    // Dynamic environment discovery - could read from config, environment variables, etc.
    var environments = new[] { "dev", "staging", "prod" };
    return $"Available environments: {string.Join(", ", environments)}. Current time: {DateTime.Now:HH:mm:ss}";
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Examples.cs#L61-L88' title='Snippet source file'>snippet source</a> | <a href='#snippet-description_method_example' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The help output will show the dynamically generated descriptions:

```bash
$ myapp.exe deploy --help
Deploy services with dynamic target discovery

Usage: myapp.exe deploy [options]

Options:

  -t | --targets (Multiple)  <TEXT>
  Available targets: app, database, cache, notifications

  -e | --environment         <TEXT>  [dev]
  Available environments: dev, staging, prod. Current time: 14:30:45
```

## Requirements

The method specified in `DescriptionMethodAttribute` must:

* Be **static**
* Take **no parameters**
* Return **string** (or `string?`)
* Be accessible (can be private, internal, or public)

The method is resolved at application startup, so any configuration errors will be caught early with clear error messages.

## Error Handling

### Multiple Description Properties

You cannot combine `DescriptionMethod` with `Description` or `DescriptionLines`:

```cs
// This will throw InvalidConfigurationException at startup
public void Bad(
    [Option(Description = "static text")]
    [DescriptionMethod(nameof(GetDescription))]
    string option) { }
```

**Error:** `Multiple description properties were set: Description, DescriptionMethod. Only one can be set.`

### Method Not Found

If the specified method doesn't exist:

```cs
public void Bad([Option, DescriptionMethod("NonExistent")] string option) { }
```

**Error:** `DescriptionMethod 'NonExistent' not found in type 'MyClass'. Method must be static with no parameters and return string.`

### Wrong Return Type

If the method doesn't return string:

```cs
public void Bad([Option, DescriptionMethod(nameof(GetNumber))] string option) { }
private static int GetNumber() => 42;
```

**Error:** `DescriptionMethod 'GetNumber' must return string but returns Int32.`

## Use Cases

### Configuration-Based Options

```cs
public void Connect(
    [Option, DescriptionMethod(nameof(GetDatabaseDescription))]
    string database)
{ }

private static string GetDatabaseDescription()
{
    // Read from configuration
    var available = ConfigurationManager.AppSettings["AvailableDatabases"];
    return $"Available databases: {available}";
}
```

### Environment-Specific Options

```cs
public void Deploy(
    [Option, DescriptionMethod(nameof(GetEnvironments))]
    string environment)
{ }

private static string GetEnvironments()
{
    var env = Environment.GetEnvironmentVariable("DEPLOYMENT_ENVS") ?? "dev,staging";
    return $"Available environments: {env}";
}
```

### Discovery-Based Options

```cs
public void Backup(
    [Option, DescriptionMethod(nameof(GetBackupTargets))]
    string target)
{ }

private static string GetBackupTargets()
{
    // Discover available backup locations at runtime
    var locations = Directory.GetDirectories("/backups");
    return $"Available: {string.Join(", ", locations.Select(Path.GetFileName))}";
}
```

## Best Practices

* **Keep it fast**: The method is called every time help is displayed. Avoid expensive operations like network calls.
* **Cache when appropriate**: If discovery is expensive, cache the results in a static field.
* **Consider localization**: The description method is called after the help provider's localization, so you may need to handle localization within your method if needed.
* **Use clear error messages**: If the method might fail, consider try-catch and return a helpful error message rather than throwing.

## Implementation Notes

* The method is resolved once at application startup and stored as a delegate
* The method is invoked lazily only when the `Description` property is accessed
* This works anywhere `argument.Description` is used, not just in help text
* The feature validates the method signature at startup, catching configuration errors early
