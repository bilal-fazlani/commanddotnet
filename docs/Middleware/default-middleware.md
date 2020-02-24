# Default middleware

To avoid the clutter and fatigue of enabling built-in middleware one-by-one, we've provided the `UseDefaultMiddleware` extension.
This method will register all middleware that does not require an external dependency and can be run with sensible defaults configurations.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware()
        .Run(args);
}
```

!!! Note
    New versions of the library may include new middleware in this method. This will be indicated as a minor semantic version update to the package.

!!! Note
    This list will never include middleware that requires an external dependency, like `FluentValidator`, `IoC` and `Humanizer`

## Excluding middleware

There will be times when you need to exclude a single middleware but want to keep all others.  

Each middleware has a corresponding `exclude...` parameter, as shown in the `UseDefaultMiddleware` method at the bottom of the page.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware(excludeVersionMiddleware: true)
        .Run(args);
}
```

## Overriding default middleware settings

Some middleware accept options to modify behavior. To provide options, first exclude the middleware from the default and then call the middleware's extension method.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware(excludePrompting: true)
        .UsePrompting(promptForMissingArguments: false)
        .Run(args);
}
```

## UseDefaultMiddleware

... at the time of this writing

```c#
public static AppRunner UseDefaultMiddleware(this AppRunner appRunner,
    bool excludeCancellationHandlers = false,
    bool excludeDebugDirective = false,
    bool excludeParseDirective = false,
    bool excludePrompting = false,
    bool excludeResponseFiles = false,
    bool excludeVersionMiddleware = false,
    bool excludeAppendPipedInputToOperandList = false)
{
    if (!excludeCancellationHandlers) appRunner.UseCancellationHandlers();
    if (!excludeDebugDirective) appRunner.UseDebugDirective();
    if (!excludeParseDirective) appRunner.UseParseDirective();
    if (!excludePrompting) appRunner.UsePrompting();
    if (!excludeResponseFiles) appRunner.UseResponseFiles();
    if (!excludeVersionMiddleware) appRunner.UseVersionMiddleware();
    if (!excludeAppendPipedInputToOperandList) appRunner.AppendPipedInputToOperandList();

    return appRunner;
}
```
