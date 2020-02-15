# Ctrl+C and CancellationToken

## TLDR, How to enable 
Enable the feature with `appRunner.UseCancellationHandlers()` or `appRunner.UseDefaultMiddleware()`.

## The problem space

Console applications should stop gracefully when the user enters `Ctrl+C` or `Ctrl+Break`. 
If your app is consuming the main thread the app will not exit right away.

Traditionally, this is solved with a following steps:

1. Create a `CancellationTokenSource` and make the `cancellationTokenSource.Token` available for the rest of the app to reference.
1. Subscribe to `Console.CancelKeyPress` and call `cancellationTokenSource.Cancel()` when triggered.  `cancellationToken.IsCancellationRequested` will then return true.
1. Check `cancellationToken.IsCancellationRequested` in any looping code and pass the token to any libraries that check it. Instead of `Thread.Sleep(...)`, use `cancellationToken.WaitHandle.WaitOne(...)` or `Task.Delay(..., cancellationToken)`

## Cancellation middleware

When enabled, the framework will:

* set the `CommandContext.AppConfig.CancellationToken` with a new token.
* register a [parameter resolver](../Extensibility/parameter-resolvers.md) for `CancellationToken`
* cancel the token on
    * `Console.CancelKepPress`
    * `AppDomain.CurrentDomain.ProcessExit`
    * `AppDomain.CurrentDomain.UnhandledException` when `UnhandledExceptionEventArgs.IsTerminating` == true

The framework checks the cancellation token before every step in the pipeline.

## Using the CancellationToken

The CancellationToken is easy to access in your commands thanks to [parameter resolvers](../Extensibility/parameter-resolvers.md). 
Simply add a parameter to your command or interceptor method.

``` c#
public void MigrateRecords(CancellationToken cancellationToken, List<int> ids)
{    
    foreach(int id in ids.TakeWhile(!cancellationToken.IsCancellationRequested))
    {
        MigrateRecord(id);
    }
}
```

!!! tip
    Remember to pass the CancellationToken to all database, web and service requests that take one.