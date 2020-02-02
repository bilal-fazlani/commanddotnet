# Interceptor methods

Interceptor methods will be executed for all commands and subcommands of a class.
Interceptor methods can define options to be provided in the shell. Interceptors cannot define operands because they are positional and positional arguments are only applicable for the target command.

Example:

Using the following commands, we can request the [mathematical base](https://simple.m.wikipedia.org/wiki/Base_(mathematics)) to use for our calculations in one location and all subcommands will use the same base. 

In the shell: 

``` bash
dotnet calculator --radix 2 Add 1 2
```

``` c#
public class Calculator
{
    private iCalculator _calculator;
    private IConsole _console;

    public Task<int> Interceptor(
        InterceptorExecutionDelegate next,
        IConsole console,
        int radix,)
    {
        _calculator = Factory.GetCalculatorFor(radix);
        _console = console;
        return next();
    }
    
    [Command(Description = "Adds two numbers")]
    public void Add(int x, int y)
    {
        _console.WriteLine(_calculator.Add(x, y));
    }

    [Command(Description = "Subtracts the second number from the first")]
    public void Subtract(int x, int x)
    {
        _console.WriteLine(_calculator.Subtract(y, y));
    }
}
```

Interceptor methods signatures must follow these rules:

* `public` accessor
* `Task<int>` return type
* contain either execution delegate: `ExecutionDelegate` or `InterceptorExecutionDelegate`.

Method name does *not* matter.  In our examples, we use `Interceptor`.

Examples:

* `public Task<int> MethodName(ExecutionDelegate next)`
* `public Task<int> MethodName(InterceptorExecutionDelegate next)`


## Inherited options

Inherited options provide a way to assign the interceptor option an an option of the executed subcommand.

Using the previous example, change `int radix` to `[Option(AssignToExecutableSubcommands = true)] int radix` 

Now in the shell: 

``` bash
dotnet calculator Add 1 2 --radix 2
```

``` c#
public class Calculator
{
    private iCalculator _calculator;
    private IConsole _console;

    public Task<int> Interceptor(
        InterceptorExecutionDelegate next,
        IConsole console,
        [Option(AssignToExecutableSubcommands = true)] int radix)
    {
        _calculator = Factory.GetCalculatorFor(radix);
        _console = console;
        return next();
    }
    
    [Command(Description = "Adds two numbers")]
    public void Add(int x, int y)
    {
        _console.WriteLine(_calculator.Add(x, y));
    }

    [Command(Description = "Subtracts the second number from the first")]
    public void Subtract(int x, int y)
    {
        _console.WriteLine(_calculator.Subtract(x, y));
    }
}
```

## Additional parameters

In addition to defining options, interceptor methods can define parameters of type:

* CommandContext
* IConsole
* CancellationToken

## Recipes

### Hooks for your commands

Wrap `return next();` in try/catch/finally statements and use the interceptor as pre and post hooks for your commands.

``` c#
    public Task<int> Interceptor(InterceptorExecutionDelegate next)
    {
        prehook();
        try
        {
            return next();
        }
        catch()
        {
            errorhook();
        }
        finally()
        {
            posthook();
        }
    }
```

### Convert to middleware

Interceptor methods are effectively locally defined middleware methods. This makes it easy to start functionality as an interceptor and convert to middleware when appropriate.

For example, the command hook pattern defined above could be converted to a middleware and used by all commands.

``` c#
public static class CommandHooksMiddlware
{
    public static AppRunner UseCommandHooks(this AppRunner appRunner)
    {
        return appRunner.Configure(c =>
            c.UseMiddleware(Middleware, MiddlewareStages.PostBindValuesPreInvoke));
    }

    private static Task<int> Middleware(CommandContext commandContext, ExecutionDelegate next)
    {
        prehook();
        try
        {
            return next(commandContext);
        }
        catch()
        {
            errorhook();
        }
        finally()
        {
            posthook();
        }
    }
}
```

### Hierarchy interaction

Interceptor methods will be run for all subcommands, including subcommands of subcommands.  If the interceptor should run only for subcommands defined in that class, follow this example to determine if the target command is for the same class instance as the current interceptor method.

``` c#
    public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext context)
    {
        if(context.InvocationPipeline.TargetCommand.Instance == this)
        {
            prehook();
        }
        return next();
    }
```