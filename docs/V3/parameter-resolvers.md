# Parameter Resolvers

Parameters defined in command and interceptor methods must a type as defined in [parameter-types](../parameter-types.md).
There are times where your command method needs to write to the `IConsole` or perform a loop while checking the `CancellationToken`.
To get these, you define an interceptor method with a `CommandContext` parameter and copy the properties to fields for use in other methods. 

This quickly gets cumbersome.

The answer... parameter resolvers, functions that, given a `CommandContext`, return an instance of a type that can be passed to a command or interceptor method.
There are three default resolvers for `CommandContext`, `IConsole` & `CancellationToken`
More can be registered like this 

``` c#
appRunner.Configure(b => b.UseParameterResolver(ctx => ctx.Services.Get<SomeClass>()));
```

These types are now available as parameters for command methods, interceptor methods and command class constructors.

``` c#
public class Calculator
{
    private iCalculator _calculator;

    public Task<int> Interceptor(
        InterceptorExecutionDelegate next, 
        [Option(Inherited = true)] int radix,
        SomeClass someClass)
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