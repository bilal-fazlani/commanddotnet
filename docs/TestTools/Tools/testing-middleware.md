# Testing Middleware

Testing middleware and the extensibility points generally requires access to the `CommandContext` and `AppConfig`.

## Accessing CommandContext

=== "RunInMem"

    ```c#
    [Test]
    public void SomeTest()
    {
        var result = new AppRunner<App>().RunInMem("some args");
        var context = result.CommandContext;
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

When your middleware can modify the method parameters, you'll want to access the invocation metadata to verify. 

There are a few [CommandContext extension methods](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/CommandContextTestExtensions.cs) to make this easier.

To get the [InvocationStep](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/InvocationStep.cs), which contains the Command, Invocation and Instance, use:

* `GetCommandInvocationStep()` to get the invocation of the command.
* `GetInterceptorInvocationStep<TInterceptorClass>()` to get the invocation of the interceptor in the specified class.

To get the [Invocation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/IInvocation.cs), which contains the Arguments, Parameters, ParameterValues and MethodInfo, use:

* `GetCommandInvocation()` to get the invocation of the command.
* `GetInterceptorInvocation<TInterceptorClass>()` to get the invocation of the interceptor in the specified class.

To get an invocation that will indicate if the method was invoked, use `GetCommandTrackingInvocation()` or `GetInterceptorTrackingInvocation<TInterceptorClass>()` and assert the `WasInvoked` property.  You will need to configure the AppRunner with `.InjectTrackingInvocations()` for this to work. A friendly error message will remind you if your forget.

To get the typed instance of class, use `GetCommandInstance<TCommandClass>` or `GetInterceptorInstance<TInterceptorClass>`.

!!! Note
    The interceptor versions of these methods will return null if the specified interceptor is not in the hieararchy of the executed command. 

The xml comments for [InvocationStep](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/InvocationStep.cs) and [Invocation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/IInvocation.cs) indicate which stage each property is loaded.

### Example

We'll start with the following WebClient with a single *Download* command and an [interceptor method](../../Extensibility/interceptors.md) to authenticate commands.  You can imagine there would be other commands like Upload, Post, Get, etc.

```c#
public class WebClient
{
    public void Authenticate(InterceptorExecutionDelegate next, 
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
                .InjectTrackingInvocations()
                .RunInMem("--user me --pwd shhhh Download http://all-the-internet.com");

            var context = result.CommandContext;

            var invocation = context.GetCommandTrackingInvocation<WebClient>();
            invocation.WasInvoked.Should().Be(true);
            invocation.ParameterValues.First().Should()
                .Be("http://all-the-internet.com");

            invocation = context.GetInterceptorTrackingInvocation<WebClient>()
            invocation.WasInvoked.Should().Be(true);
            invocation.ParameterValues.Should()
                .Be(new []{"me", "shhhh"});
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public void SomeTest()
    {
        new AppRunner<WebClient>()
            .InjectTrackingInvocations()
            .Verify(new Scenario
            {
                When = {Args = "--user me --pwd shhhh Download http://all-the-internet.com"},
                Then =
                {
                    AssertContext = ctx => 
                    {
                        var invocation = ctx.GetCommandTrackingInvocation<WebClient>();
                        invocation.WasInvoked.Should().Be(true);
                        invocation.ParameterValues.First().Should()
                            .Be("http://all-the-internet.com");

                        invocation = ctx.GetInterceptorTrackingInvocation<WebClient>()
                        invocation.WasInvoked.Should().Be(true);
                        invocation.ParameterValues.Should()
                            .Be(new []{"me", "shhhh"});
                    }
                }
            });
    }
    ```

!!! Tip
    CommandDotNet has an extensive set of tests like this to verify all of it's features.
    You should only need to test invocations when verifying custom middleware.