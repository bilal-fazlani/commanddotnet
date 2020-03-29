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

For additional debugging info, configuration can be printed with `appRunner.ToString()`.  See the [Command Logger > AppConfig](command-logger.md#appconfig) section for an example.