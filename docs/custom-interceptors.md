# Custom interceptors before method execution

Invoking the command method can be customized by implementing ICommandInvoker and registering the an instance with AppRunner.WithCommandInvoker. The WithCommandInvoker method provides the default implementation so it can be wrapped using the [Decorator Pattern](https://en.wikipedia.org/wiki/Decorator_pattern).

```c#
static int Main(string[] args)
{
     AppRunner<CustomCommandInvokerApp> appRunner = new AppRunner<CustomCommandInvokerApp>().WithCommandInvoker(inner => new CustomCommandInvoker(inner));
     return appRunner.Run(args);
}

public class CustomCommandInvoker : ICommandInvoker
{
    private readonly ICommandInvoker _inner;

    public CustomCommandInvoker(ICommandInvoker inner)
    {
        _inner = inner;
    }

    public object Invoke(CommandInvocation commandInvocation)
    {
        // Your custom code here
        return _inner.Invoke(commandInvocation);
    }
}
```