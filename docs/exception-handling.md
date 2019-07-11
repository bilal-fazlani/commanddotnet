Any exception that is thrown from the method or constructor is thrown as is. You can catch it over the `AppRunner<T>.Run()` method otherwise exception will be unhandled and application will crash.

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
