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


## Testing Console Apps

One of the perks of using this framework is that commands are just methods and methods are easily unit tested. Most of your tests can be unit tests, as is best-practice.

These tools enable you to provide end-to-end testing as if running the app in a console.

!!! Note
    These test tools are used to test all of the CommandDotNet features.<br/>They are well suited to testing middleware and other extensibility components. 

## Two testing patterns

The tool provides extension methods facilitating two testing patterns.

=== "Standard"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem("List aaa bbb", pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.OutputShouldBe(@"aaa
    bbb
    ccc
    ddd
    ");
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

=== "BDD"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    Given = { PipedInput = new[] { "ccc", "ddd" } },
                    WhenArgs = "List aaa bbb",
                    Then =
                    {
                        Output = @"aaa
    bbb
    ccc
    ddd
    "
                    }
                });
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

With both patterns use an extension method that runs a fully configured AppRunner.

### Standard

The standard style uses the `RunInMem` extension method which will  

* Configure CommandDotNet logging
* Configure TestConsole with 
    * pipedInput
    * onReadLine
    * promptResponder
* Execute appRunner.Run(args)
* If enabled, prints
    * Console Output
    * CommandContext
    * AppConfig
* Return AppRunnerResult with
    * ExitCode
    * TestConsole
    * TestCaptures
    * CommandContext

See [Run in Memory](run-in-mem.md) for more details.

### BDD

The BDD style uses the `Verify` extension method which wraps `RunInMem` with assertions defined in a BDD manner using [object initializers](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) for some syntactic goodness.

* Assert
  * ExitCode
  * Error Message
  * Console Output
  * TestCaptures
* On Error
  * Capture & Return ExitCode 1

See [BDD](bdd.md) for more details.

This approach works well with BDD frameworks like [SpecFlow](https://specflow.org/) where scenarios can be defined in other sources and mapped to code. 

## Testing your application

When testing an application, use the same method to generate and configure the AppRunner for the console and tests. In this example, the `GetAppRunner()` method is made public so that tests can   

```c#
public class Program
{
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            return GetAppRunner().Run(args);
        }

        public static AppRunner GetAppRunner()
        {    
            new AppRunner<Git>()
                .UseDefaultMiddleware()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation();
        }
}
```

```c#
[TestFixture]
public class ProgramTests
{
    [Test]
    public void Checkout_NewBranch_WithoutBranchFlag_Fails()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                WhenArgs = "checkout lala",
                Then = { 
                    Output = "error: pathspec 'lala' did not match any file(s) known to git" 
                }
            });
    }

    [Test]
    public void Checkout_NewBranch_BranchFlag_Succeeds()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                WhenArgs = "checkout -b lala",
                Then = { 
                    Output = "Switched to a new branch 'lala'" 
                }
            });
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

### [TestCaptures](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestCaptures.cs)

`TestCaptures` can be added as a property to a test app and will be automatically injected by the test framework.

This is useful for testing middleware and other extensions that can populate or modify arguments, by allowing you to capture those arguments and assert them in tests.

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)

