# Test Tools

One of the perks of using this framework is that commands are just methods and methods are easily unit tested. Most of your tests can be unit tests, as is best-practice.

These tools enable you to provide end-to-end testing with the same experience as the console. 

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

The framework is ideal for end-to-end testing and testing middleware components.

## Testing Middleware

CommandDotNet features are tested extensively using these test tools and so they are well suited to testing middleware and other extensibility components. 

## Two testing patterns

The tool provides extension methods facilitating two different testing patterns.

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

=== "BDD Style"

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

### Standard Style

lala descriptions

### BDD Style

lala descriptions

This approach works well with BDD frameworks like [SpecFlow](https://specflow.org/) where scenarios can be defined in other sources and mapped to code. 

### Program

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

