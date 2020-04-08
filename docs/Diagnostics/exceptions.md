# Exceptions

CommandDotNet rethrows all exceptions raised by the application code. 

Wrap the call to `appRunner.Run(args)` to handle global exceptions.

```c#
var appRunner = new AppRunner<Calculator>();
try
{
    return appRunner.Run(args);
}
catch(MyBusinessException ex)
{
    Console.WriteLine(ex.Message);

#if DEBUG
    // log appRunner config
    Console.WriteLine(appRunner.ToString())
#endif
}
```

CommandDotNet internal errors are generally captured and return error code 1.

Validation middleware will generally return error code 2.

Middlware should print error messages and return an error code instead of raising exceptions.

## Additional Diagnostic Info

### CommandContext from the exception

Exceptions that escape appRunner will contain the CommandContext in the Exception.Data property.

Use the `ex.GetCommandContext()` extension method to get the instance. Use the namespace `CommandDotNet.Diagnostics`

With the context, you can `.PrintHelp()` or `ParserReport.Report` to log details of the request.

### Printing config information

Configuration can be printed with `appRunner.ToString()`.  See the [Command Logger > AppConfig](command-logger.md#appconfig) section for an example.

### Full example

```c#
var appRunner = new AppRunner<Calculator>();
try
{
    return appRunner.Run(args);
}
catch(MyBusinessException ex)
{
    Console.WriteLine(ex.Message);

#if DEBUG

    var ctx = ex.GetCommandContext();

    // log the parse arguments
    Console.WriteLine(ParseReporter.Report(ctx));

    // print help for the target command
    Console.WriteLine(ctx.PrintHelp());

    // log appRunner config
    Console.WriteLine(appRunner.ToString())
#endif
}
```