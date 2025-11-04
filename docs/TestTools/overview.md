# Overview

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

However, you may find occassions where you'd like to write end-to-end tests to confirm your app is configured correctly and will work as expected from the console. 

These tools enable you to provide end-to-end testing as if running the app in a console.

!!! Note
    These test tools are used to test all of the CommandDotNet features.<br/>They are well suited to testing middleware and other extensibility components. 

## Testing the AppRunner

The tool provides two extension methods to execute an AppRunner in memory and collect the results.

=== "RunInMem"

    <!-- snippet: testtools_runinmem_example -->
    ```cs
    public static void RunInMem_Example()
    {
        var result = new AppRunner<App>()
            .RunInMem("Add 2 3");

        // result.ExitCode.Should().Be(0);
        // result.Console.Out.Should().Contain("5");
    }

    private class App
    {
        public void Add(IConsole console, int x, int y) =>
            console.WriteLine(x + y);
    }
    ```
    <sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/TestTools/TestTools_Examples.cs#L12-L27' title='Snippet source file'>snippet source</a></sup>
    <!-- endSnippet -->

=== "BDD Verify"

    <!-- snippet: testtools_bdd_verify_example -->
    ```cs
    public static void BDD_Verify_Example()
    {
        new AppRunner<App>()
            .Verify(new Scenario
            {
                When = 
                {
                    Args = "Add 2 3"
                },
                Then =
                {
                    Output = "5"
                }
            });
    }
    ```
    <sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/TestTools/TestTools_Examples.cs#L29-L45' title='Snippet source file'>snippet source</a></sup>
    <!-- endSnippet -->

[RunInMem](Harness/run-in-mem.md) will run the runner and collect results. Assertions will need to be executed after.

[Verify](Harness/bdd.md) wraps `RunInMem` with declarative BDD style setup and assertions.

!!! Tip
    If you're using the [.UseDefaultMiddleware()](../OtherFeatures/default-middleware.md) method, testing with either of these will help identify behavior changes on upgrade due to new opt-in features.

## Testing your application

### Using the same configuration

When testing an application, use the same method to generate and configure the AppRunner for the console and tests. In this example, the `Program.GetAppRunner()` method is introduced and made public 
so tests get an AppRunner configured exactly as it will be when run from the console. If you have additional configuration, such as an IoC container, be sure it's included in this method.

```cs
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

```cs
[TestFixture]
public class ProgramTests
{
    [Test]
    public void Checkout_NewBranch_BranchFlag_Succeeds()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                When = { Args = "checkout -b lala" },
                Then = { 
                    Output = "Switched to a new branch 'lala'" 
                }
            });
    }
}
```

### AppInfo and AppName generation in tests

CommandDotNet uses `Assembly.GetEntryAssembly()` and `Process.GetCurrentProcess().MainModule` to generate the AppInfo and AppName used in the auto-generated help.

While ideal when executing the app, this is not deterministic when executing from a test runner.

For example, 

* when run using `dotnet test`, the AppName will be "dotnet testhost.dll"
* when run using the Resharper test runner, the AppName will be "ReSharperTestRunner64.exe"

See [Deterministic AppName for tests](Tools/deterministic-appinfo.md) for tips to work around this.

## Included test tools

The framework includes the following tools that can be used independently of the BDD Framework.

### [TestConsole](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestConsole.cs)

* capture output for assertions
* provide piped input
* handle ReadLine and ReadKey
* covers most members of System.Console

### [TestEnvironment](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestEnvironment.cs)

* mock values for IEnvironment, an interface for System.Environment and System.Runtime.InteropServices.RuntimeInformation
* make tests more reliable across machines and operating systems

### [TempFiles](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TempFiles.cs)

* Creates temp files, give content and receive a file path.
* Removes created files on dispose
* Examples: [ResponseFileTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/ResponseFileTests.cs)

### [TestDependencyResolver](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestDependencyResolver.cs) 

```cs
new AppRunner<App>()
    .UseDependencyResolver(new TestDependencyResolver { dbSvc, httpSvc })
    .VerifyScenario(scenario);
```

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)

