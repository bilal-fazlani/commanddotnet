# Testing your app

Traditionally, one of the problems with writing console apps is automated testing of the apps.
CommandDotNet solves this with our [Test Tools](../TestTools/overview.md).

We make it easy to test your app as if you're entering the commands in the console.

The first step is to get access to the AppRunner the program is using so your tests are testing the application as it is configured.

Let's extract the configuration into a public static property

<!-- snippet: getting-started-400-calculator -->
<a id='snippet-getting-started-400-calculator'></a>
```c#
static int Main(string[] args) => AppRunner.Run(args);

public static AppRunner AppRunner => new AppRunner<Program>();
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_400_Testing.cs#L13-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-400-calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Now the tests can use `Program.AppRunner` for all tests.

CommandDotNet supports two different test patterns:

### Standard

<!-- snippet: getting-started-400-calculator-add-command-tests -->
<a id='snippet-getting-started-400-calculator-add-command-tests'></a>
```c#
[Test]
public void Given2Numbers_Should_OutputSum()
{
    var result = Program.AppRunner.RunInMem("Add 40 20");
    result.ExitCode.Should().Be(0);
    result.Console.OutText().Should().Be("60");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_400_Testing.cs#L27-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-400-calculator-add-command-tests' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### BDD Style

<!-- snippet: getting-started-400-calculator-add-command-tests-bdd -->
<a id='snippet-getting-started-400-calculator-add-command-tests-bdd'></a>
```c#
[Test]
public void Given2Numbers_Should_OutputSum() =>
    Program.AppRunner.Verify(new Scenario
    {
        When = { Args = "Add 40 20" },
        Then = { Output = "60" }
    });

[Test]
public void GivenANonNumber_Should_OutputValidationError() =>
    Program.AppRunner.Verify(new Scenario
    {
        When = { Args = "Add a 20" },
        Then =
        {
            ExitCode = 2, // validations exit code = 2
            Output = "'a' is not a valid Number"
        }
    });
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_400_Testing.cs#L41-L61' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-400-calculator-add-command-tests-bdd' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See [Test Tools](../TestTools/overview.md) in the Testing help section for more, such as testing prompts and piped input. 
