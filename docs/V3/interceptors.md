# Interceptor methods

Interceptor methods provide a way to define a method that will be executed for all subcommands, as well as the current command if it contains a default method.
Interceptor methods can define options to be provided in the shell.  Interceptors cannot define arguments because they are positional and positional arguments are not allowed when a subcommand is requested.

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

    public Task<int> Interceptor(
        InterceptorExecutionDelegate next, 
        int radix)
    {
        _calculator = Factory.GetCalculatorFor(radix);
        return next();
    }
    
    [Command(Description = "Adds two numbers")]
    public void Add(int x, int y, IConsole console)
    {
        console.WriteLine(_calculator.Add(x, y));
    }

    [Command(Description = "Subtracts the second number from the first")]
    public void Subtract(int x, int x, IConsole console)
    {
        console.WriteLine(_calculator.Subtract(y, y));
    }
}
```

## Inherited options

Inherited options provide a way to specify the interceptor option in the executed subcommand.

Using the previous example, change `int radix` to `[Option(Inherited = true)] int radix` 

Now in the shell: 

``` bash
dotnet calculator Add 1 2 --radix 2
```

``` c#
public class Calculator
{
    private iCalculator _calculator;

    public Task<int> Interceptor(
        InterceptorExecutionDelegate next, 
        [Option(Inherited = true)] int radix)
    {
        _calculator = Factory.GetCalculatorFor(radix);
        return next();
    }
    
    [Command(Description = "Adds two numbers")]
    public void Add(int value1, int value2, IConsole console)
    {
        console.WriteLine(_calculator.Add(value1, value2));
    }

    [Command(Description = "Subtracts the second number from the first")]
    public void Subtract(int value1, int value2, IConsole console)
    {
        console.WriteLine(_calculator.Subtract(value1, value2));
    }
}
```

## Additional parameters

In addition to defining options, interceptor methods can define parameters of type:

* CommandContext
* IConsole
* CancellationToken

## Hooks for your commands

Wrap `return next();` in try/catch/finally statements and use the interceptor as pre and post hooks for your commands.