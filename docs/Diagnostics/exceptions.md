# Exceptions

CommandDotNet rethrows all exceptions raised by the application code. The app developer is responsible for ensuring they are logged and displayed to the user.

CommandDotNet throws two types of exceptions

* `ValueParsingException`: indicates user error and is localizable.
* `InvalidConfigurationException`: indicates exceptions that can be fixed by developers. These exceptions are not a result of user error and are thus not localizable. It is expected the developer will discover these during testing.

## Two patterns for handling exceptions

### UseErrorHandler delegate

<!-- snippet: exceptions_use_error_handler_delegate -->
<a id='snippet-exceptions_use_error_handler_delegate'></a>
```cs
static int Main(string[] args)
{
    return new AppRunner<Program>()
        .Configure(b =>
        {
            // some other setup that could throw exceptions
            // i.e. configure containers, load configs, register custom middleware
        })
        .UseErrorHandler((ctx, ex) =>
        {
            (ctx?.Console.Error ?? Console.Error).WriteLine(ex.Message);
            return ExitCodes.ErrorAsync.Result;
        })
        .Run(args);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Exceptions.cs#L55-L71' title='Snippet source file'>snippet source</a> | <a href='#snippet-exceptions_use_error_handler_delegate' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This delegate will be used for any errors thrown within `AppRunner.Run` or `AppRunner.RunAsync`

#### When to use

If you plan to test your app using the [TestTools](../TestTools/overview.md) approach [mentioned here](../TestTools/overview.md#testing-your-application).  

All application configuration should occur within `AppRunner.Configure` or other middleware extensions to ensure the configuration is executed within `AppRunner.Run` or `AppRunner.RunAsync`

This is the recommended approach.

### Try/Catch AppRunner.Run

Wrap the call to `appRunner.Run(args)` to handle global exceptions.

<!-- snippet: exceptions_try_catch -->
<a id='snippet-exceptions_try_catch'></a>
```cs
static int Main(string[] args)
{
    try
    {
        // some other setup that could throw exceptions
        // i.e. configure containers, load configs, register custom middleware

        return new AppRunner<Program>().Run(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        return ExitCodes.ErrorAsync.Result;
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Exceptions.cs#L87-L103' title='Snippet source file'>snippet source</a> | <a href='#snippet-exceptions_try_catch' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Exit Codes

CommandDotnet has the following pre-defined exit codes

<!-- snippet: ExitCodes_class -->
<a id='snippet-exitcodes_class'></a>
```cs
public static class ExitCodes
{
    public static int Success => 0;
    public static int Error => 1;
    public static int ValidationError => 2;
        
    public static Task<int> SuccessAsync => Task.FromResult(Success);
    public static Task<int> ErrorAsync => Task.FromResult(Error);
    public static Task<int> ValidationErrorAsync => Task.FromResult(ValidationError);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ExitCodes.cs#L5-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-exitcodes_class' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

CommandDotNet internal errors are generally captured and return `ExitCodes.Error` (1).

Validation middleware will generally return `ExitCodes.ValidationError` (2).

When middlware causes an exception, it should print error messages and return an error code instead of raising the exception.

## Additional Diagnostic Info

You can see each in use in the example at the end.

### CommandContext from the exception

Exceptions that escape appRunner will contain the CommandContext in the Exception.Data property.

Use the `ex.GetCommandContext()` extension method to get the instance. Use the namespace `CommandDotNet.Diagnostics`

### Print exceptions

With the exception, use one of the `ex.Print(...)` extension methods to also print exception properties, `.Data` values, and stack trace.

Here are the core parameters

<!-- snippet: exception_print_parameters -->
<a id='snippet-exception_print_parameters'></a>
```cs
bool includeProperties = false,  // print exception properties
bool includeData = false,        // print values from ex.Data dictionary
bool includeStackTrace = false,  // print stack trace
bool excludeTypeName = false     // do not print exception type name
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Diagnostics/ExceptionExtensions.cs#L81-L86' title='Snippet source file'>snippet source</a> | <a href='#snippet-exception_print_parameters' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Print command help

With the context, use the `ctx.PrintHelp()` extension method. 
If the error occurred after the parser determined the target command, prints help for the target command. 
If a target command was not determined, prints help for the root command.

Use this for user related errors where it could be helpful for them to see the help again to change their input.

### Printing config information

Configuration can be printed with `appRunner.ToString()`.  See the [Command Logger > AppConfig](command-logger.md#include-appconfig) section for an example.

Alternatively, `CommandLogger.Log(ctx)` can be used to output CommandLogger for exception handling.

This can be very helpful for troublshooting, but could also be overly verbose for command errors. Consider your user base carefully when using this pattern.

## Full example

Here's an example using all the features from above, which is likely overkill for most apps, but shown so you can see what's available.

<!-- snippet: exceptions_cmdlog_error_handler -->
<a id='snippet-exceptions_cmdlog_error_handler'></a>
```cs
static int Main(string[] args) => AppRunner.Run(args);

public static AppRunner AppRunner =>
    new AppRunner<Program>()
        .UseErrorHandler(ErrorHandler);

private static int ErrorHandler(CommandContext? ctx, Exception exception)
{
    var errorWriter = (ctx?.Console.Error ?? Console.Error);
    exception.Print(errorWriter.WriteLine,
        includeProperties: true,
        includeData: true,
        includeStackTrace: false);

    // use CommandLogger if it has not already logged for this CommandContext
    if (ctx is not null && !CommandLogger.HasLoggedFor(ctx))
    {
        CommandLogger.Log(ctx,
            writer: errorWriter.WriteLine,
            includeSystemInfo: true,
            includeMachineAndUser: true,
            includeAppConfig: false
        );
    }

    // print help for the target command or root command
    // if the exception occurred before a command could be parsed
    ctx?.PrintHelp();

    return ExitCodes.ErrorAsync.Result;
}

public void Throw(string message)
{
    throw new ArgumentException(message, nameof(message))
    {
        Data = { { "method", nameof(Exceptions.Throw) } }
    };
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Exceptions.cs#L119-L159' title='Snippet source file'>snippet source</a> | <a href='#snippet-exceptions_cmdlog_error_handler' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: exceptions_throw_cmdlog -->
<a id='snippet-exceptions_throw_cmdlog'></a>
```bash
$ example.exe Throw yikes
System.ArgumentException: yikes (Parameter 'message')
Properties:
  Message: yikes (Parameter 'message')
  ParamName: message
Data:
  method: Throw

***************************************
Original input:
  Throw yikes

command: Throw

arguments:

  message <Text>
    value: yikes
    inputs: yikes
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
Machine       = my-machine
Username      = \my-machine\username
***************************************

Usage: example.exe Throw <message>

Arguments:

  message  <TEXT>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/exceptions_throw_cmdlog.bash#L1-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-exceptions_throw_cmdlog' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The sections between `***************************************` lines are from the CommandLogger.
