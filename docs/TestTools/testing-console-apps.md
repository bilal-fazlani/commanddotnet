# Testing Console Apps

One of the perks of using this framework is that commands are just methods and methods are easily unit tested. Most of your tests can be unit tests, as is best-practice.

These tools enable you to provide end-to-end testing as if running the app in a console.

If you're using the [.UseDefaultMiddleware()](../OtherFeatures/default-middleware.md) method, testing as this layer will help identify bugs on upgrade due to new opt-in features.

!!! Note
    These test tools are used to test all of the CommandDotNet features.<br/>They are well suited to testing middleware and other extensibility components. 

## Testing the AppRunner

The tool provides two extension methods to execute an AppRunner in memory and collect the results.

=== "RunInMem"

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

=== "BDD Verify"

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

[RunInMem](run-in-mem.md) will run the runner and collect results. Assertions will need to be executed after.

[Verify](bdd.md) wraps `RunInMem` with declarative BDD style setup and assertions.

## Testing your application

When testing an application, use the same method to generate and configure the AppRunner for the console and tests. In this example, the `GetAppRunner()` method is made public so tests can verify the exact config used in the application.

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