# Test Tools

CommandDotNet features are tested using a BDD framework built specificly for this framework.

The framework is ideal for testing middleware components and end-to-end testing to verify configuration.

nuget package: [CommandDotNet.TestTools](https://www.nuget.org/packages/CommandDotNet.TestTools)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.TestTools
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.TestTools
    ```
## The BDD framework

More details about Scenario testing can be found in our readme files [here](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/README.md)
and [here](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/README.md)


```c#
public class PipedInputTests
{
    [Test]
    public void PipedInput_Should_UnionWithUserSuppliedValues()
    {    
        new AppRunner<App>()
            .AppendPipedInputToOperandList()
            .VerifyScenario(
                new Scenario
                {
                    Given = {PipedInput = new[] {"ccc", "ddd"}},
                    WhenArgs = $"ListArgs aaa bbb",
                    Then =
                    {
                        Result = @"aaa
bbbb
cccc
dddd
"
                    }
                });
    } 

    public class App
    {
        public void ListArgs(List<string> args)
        {
            foreach(arg in args)
            {
                console.Out.WriteLine(arg);
            }
        }
    }
}
```

## Included test tools

The framework includes the following tools that can be used independently of the BDD Framework.

### [PromptResponder](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/Prompts/PromptResponder.cs)

* Respond to and verify prompt requests, based on OnReadKey. Works with `IPrompter` middleware.
* Examples: [prompting tests](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.Tests/FeatureTests/Prompting)

### [TestConsole](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestConsole.cs)

* capture output for assertions
* provide piped input
* handle ReadLine and ReadToEnd

### [TempFiles](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TempFiles.cs)

* Creates temp files, give content and receive a file path.
* Removes created files on dispose
* Examples: [ResponseFileTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/ResponseFileTests.cs)

### [TestDependencyResolver](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestDependencyResolver.cs) 

```c#
new AppRunner<App>()
    .UseDependencyResolver(new TestDependencyResolver { dbSvc, httpSvc })
    .VerifyScenario(scenario);
```

### [TestOutputs](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestOutputs.cs)

`TestOutputs` can be added as a property to a test app and will be automatically injected by the test framework.

This is useful for testing middleware and other extensions that can populate or modify arguments, by allowing you to capture those arguments and assert them in tests.

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)

