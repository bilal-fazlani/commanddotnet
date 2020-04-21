# Testing Middleware

Testing middleware and the extensibility points generally requires access to the `CommandContext` and `AppConfig`.

## Accessing CommandContext

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void SomeTest()
        {
            var result = new AppRunner<App>().RunInMem("some args");
            var context = result.CommandContext;
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public void SomeTest()
    {
        var result = new AppRunner<App>()
            .Verify(new Scenario
            {
                When = {Args = "some args"},
                Then =
                {
                    // Action<CommandContext> to perform assertions
                    AssertContext = ctx => ...
                }
            });
        var context = result.CommandContext;
    }
    ```

Both `RunInMem` and `Verify` return a CommandContext to use for further assertions.

Verify also has the Scenario.Then.AssertContext callback for a more fluid syntax approach.

## Accessing parameter values

When your middleware can modify the method parameters, 
you'll want to access the invocation metadata to verify. 

There are a few CommandContext extension methods that make this simple.

To get the [InvocationStep](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/InvocationStep.cs) `{Command, Invocation, Instance}` use:

* `GetCommandInvocationStep()` to get the invocation of the command.
* `GetInterceptorInvocationStep<TInterceptorClass>()` to get the invocation of the interceptor in the specified class.

To get the [Invocation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/IInvocation.cs) `{Arguments, Parameters, ParameterValues, MethodInfo}` use:

* `GetCommandInvocation()` to get the invocation of the command.
* `GetInterceptorInvocation<TInterceptorClass>()` to get the invocation of the interceptor in the specified class.

The source files are linked. The comments indicate which stage each property is loaded.

Let's look at an example of how to use these.

We'll start with the following WebClient with a single *Download* command and an [interceptor method](../../Extensibility/interceptors.md)

```c#
public class WebClient
{
    public void Auth(InterceptorExecutionDelegate next, 
                            string user, string pwd)
    {
        _client.Connect(user, pwd);
        return next();
    }

    public void Download(string url, IConsole console)
    {
        _client.Find(filter).ForEach(i => console.Out.WriteLine(i.Id));
    }
}
```

And then we can assert some values passed to the methods like this.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void SomeTest()
        {
            var result = new AppRunner<WebClient>()
                .RunInMem("--user me --pwd shhhh Download http://all-the-internet.com");
            var context = result.CommandContext;

            context.GetCommandInvocation().ParameterValues
                   .First().Should().Be("http://all-the-internet.com");

            context.GetInterceptorInvocation<WebClient>().ParameterValues
                   .Should().Be(new []{"me", "shhhh"});
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public void SomeTest()
    {
        new AppRunner<WebClient>()
            .Verify(new Scenario
            {
                When = {Args = "--user me --pwd shhhh Download http://all-the-internet.com"},
                Then =
                {
                    AssertContext = ctx => 
                    {
                        ctx.GetCommandInvocation().ParameterValues
                           .First().Should().Be("http://all-the-internet.com");

                        ctx.GetInterceptorInvocation<WebClient>().ParameterValues
                           .Should().Be(new []{"me", "shhhh"});
                    }
                }
            });
    }
    ```

!!! Tip
    CommandDotNet has an extensive set of tests like this to verify all of it's features.
    You only need to test invocations when verifying your custom middleware.
