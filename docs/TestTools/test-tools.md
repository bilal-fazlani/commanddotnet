# Test Tools

#### TLDR, How to enable 

nuget package: [CommandDotNet.TestTools](https://www.nuget.org/packages/CommandDotNet.TestTools)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.TestTools
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.TestTools

    ```
## Patterns

The *Testing > Patterns* section of the docs covers extension methods and accompanying patterns used to test this framework and that are available for you to use, including how to test piping and prompting.

## Included test tools

The framework includes the following tools that can be used independently of the BDD Framework.

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

### [TestCaptures](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestCaptures.cs)

`TestCaptures` can be added as a property to a test app and will be automatically injected by the test framework.

This is useful for testing middleware and other extensions that can populate or modify arguments, by allowing you to capture those arguments and assert them in tests.

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)

