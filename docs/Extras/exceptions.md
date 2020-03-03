# Exceptions

CommandDotNet rethrows all exceptions raised by the application code. 

Wrap the call to `appRunner.Run(args)` to handle global exceptions.

```c#
try
{
    AppRunner<Calculator> appRunner = new AppRunner<Calculator>();
    return appRunner.Run(args);
}
catch(MyBusinessException ex)
{
    Console.WriteLine(ex.Message);
}
```

CommandDotNet internal errors are generally captured and return error code 1.

Validation middleware will generally return error code 2.

Middlware should print error messages and return an error code instead of raising exceptions.

