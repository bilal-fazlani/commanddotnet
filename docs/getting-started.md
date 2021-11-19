# Getting Started

## What is CommandDotNet?

**CommandDotNet is modern framework for building modern CLI apps**

Out of the box support for commands, sub-commands, validations, dependency injection, 
piping and streaming, enums & custom types, typo suggestions, prompting, passwords, response files and much more!

Favors [POSIX conventions](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html)

Includes [test tools](TestTools/overview.md) used by the framework to test all features of the framework.

Modify and extend the functionality of the framework through configuration and middleware.
## Support

For bugs, [create an issue](https://github.com/bilal-fazlani/commanddotnet/issues/new)

For questions and feature requests, start [a discussion](https://github.com/bilal-fazlani/commanddotnet/discussions)

## Credits 🎉

Special thanks to [Drew Burlingame](https://github.com/drewburlingame) for continuous support and contributions

## Installation

CommandDotNet can be installed from [nuget.org](https://www.nuget.org/packages/CommandDotNet/)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet
    ```


=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet
    ```

## Let's build a calculator

Let's say you want to create a calculator console application which can perform 2 operations: Addition & Subtraction

Begin by creating the commands:
<!-- snippet: getting_started_calculator -->
<a id='snippet-getting_started_calculator'></a>
```c#
public class Program
{
    static int Main(string[] args) => 
        new AppRunner<Program>().Run(args);

    public void Add(int x, int y) => 
        Console.WriteLine(x + y);

    public void Subtract(int x, int y) => 
        Console.WriteLine(x + y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/DocExamples/GettingStarted/Eg1_Minumum/Program.cs#L5-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

That's it. You now have an applciation with two commands. Let's see about how we can call it from command line.

Assuming our application's name is `calculator.dll`, let's run this app from command line using dotnet.
First we'll check out the auto-generated help.

```bash
~
$ dotnet calculator.dll --help
Usage: dotnet calculator.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calculator.dll [command] --help" for more information about a command.
```

```bash
~
$ dotnet calculator.dll Add --help
Usage: dotnet calculator.dll Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```

Let's try it out by adding two numbers

```bash
~
$ dotnet example.dll Add 40 20
60
```

!!! Note
    CommandDotNet also supports running your application as an .exe and as a dotnet tool.

## Let's improve the help

The help could be more helpful. We can add descriptions.

<!-- snippet: getting_started_calculator_with_descriptions -->
<a id='snippet-getting_started_calculator_with_descriptions'></a>
```c#
[Command(Description = "Performs mathematical calculations")]
public class Program
{
    static int Main(string[] args) => 
        new AppRunner<Program>().Run(args);

    [Command("Sum", Description = "Adds two numbers")]
    public void Add(int x, int y) => 
        Console.WriteLine(x + y);

    [Command(Description = "Subtracts two numbers")]
    public void Subtract(int x, int y) => 
        Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/DocExamples/GettingStarted/Eg2_Descriptions/Program.cs#L5-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_with_descriptions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Descriptions can also be added to the arguments and extended help can be added for commands to appear below all other help.

Notice we also changed the name of the Add command to Sum.

Let's see how the help appears now.

```bash
~
$ dotnet example.dll --help
Performs mathematical calculations

Usage: dotnet example.dll [command]

Commands:

  Subtract  Subtracts two numbers
  Sum       Adds two numbers

Use "dotnet example.dll [command] --help" for more information about a command.

```

Descriptions are not required but can be very useful depending upon the complexity of your app and the audience.

Now let's see help for the _Add_ command.

```bash
~
$ dotnet example.dll Add --help
Adds two numbers

Usage: dotnet example.dll Add Sum <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```

Here we see arguments for addition and their type.  See the [Arguments](Arguments/arguments.md) section for more options to configure arguments.

## Let's add some tests

One of the problems with writing console apps is being able to automate the testing.
CommandDotNet solves this with our [Test Tools](TestTools/overview.md).

We make it easy to test your app as if you're entering the commands in the console.

We support two different patterns:

### Standard

<!-- snippet: getting_started_calculator_add_command_tests -->
<a id='snippet-getting_started_calculator_add_command_tests'></a>
```c#
[Test]
public void Given2Numbers_Should_OutputSum()
{
    // lala
    var result = Program.AppRunner.RunInMem("Add 40 20");
    result.ExitCode.Should().Be(0);
    result.Console.OutText().Should().Be("60");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example.Tests/DocExamples/GettingStarted/AddCommandTests.cs#L11-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_add_command_tests' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### BDD Style

<!-- snippet: getting_started_calculator_add_command_tests_bdd -->
<a id='snippet-getting_started_calculator_add_command_tests_bdd'></a>
```c#
[Test]
public void Given2Numbers_Should_OutputSum() =>
    Program.AppRunner.Verify(new Scenario
    {
        When = { Args = "Add 40 20" },
        Then = { Output = "60" }
    });

[Test]
public void GivenANonNumber_Should_OutputError() =>
    Program.AppRunner.Verify(new Scenario
    {
        When = { Args = "Add a 20" },
        Then =
        {
            ExitCode = 1,
            Output = "'a' is not a valid Number"
        }
    });
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example.Tests/DocExamples/GettingStarted/AddCommandTests.cs#L25-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_add_command_tests_bdd' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See [Test Tools](TestTools/overview.md) in the Testing help section for more details 

## Let's calculate a sum from a stream of piped arguments

<!-- snippet: getting_started_pipes -->
<a id='snippet-getting_started_pipes'></a>
```c#
public class Program
{
    static int Main(string[] args) =>
        new AppRunner<Program>().Run(args);

    public void Range(int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count))
        {
            Console.WriteLine(i);
            if (sleep > 0)
            {
                Thread.Sleep(sleep);
            }
        }
    }

    public void Sum(IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values)
        {
            Console.WriteLine(total+=value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/DocExamples/GettingStarted/Eg5_Pipes/Program.cs#L8-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_pipes' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here we've converted the arguments for Sum into an IEnumerable<int> and added a Range command.
You've probably noticed these commands wrap LINQ methods of the same name. 
We've added an optional sleep option to Range to better mimic a long running stream. 

We could have used List<int>, int[], or any other collection type. 
Using IEnumerable<T> allows the command to start processing before the stream has completed.

Very few console frameworks make it this easy to write streaming console tools like this.

Let's see it in action:

```bash
~
$ dotnet example.dll Range 1 4 10000 | dotnet example.dll Sum
1
3
6
10
```

Range sleeps for 10 seconds after outputtting each value and Sum immediatly outputs the new sum.

## Let's handle Ctrl+C

The above command will take 40 seconds to execute. The way it's currently configured, we have no way to exit early.

With console applications, the standard pattern is to exit the app when Ctrl+C is pressed.

<!-- snippet: getting_started_ctrlc -->
<a id='snippet-getting_started_ctrlc'></a>
```c#
public class Program
{
    static int Main(string[] args) =>
        new AppRunner<Program>()
            .UseCancellationHandlers()
            .Run(args);

    public void Range(CancellationToken ct, int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count).UntilCancelled(ct, sleep))
        {
            Console.WriteLine(i);
        }
    }

    public void Sum(CancellationToken ct, IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values.ThrowIfCancelled(ct))
        {
            Console.WriteLine(total+=value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/DocExamples/GettingStarted/Eg6_CtrlC/Program.cs#L8-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_ctrlc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Again, CommandDotNet makes this very easy. Configure the app with `UseCancellationHandlers()` and a `CancellationToken` can be injected into your commands. Use either of the two handy extension methods `UntilCancelled` or `ThrowIfCancelled` to exit an enumeration early.

## Opt-In to additional features

In the `Program.Main`, we configured the app with the basic feature set.

<!-- snippet: getting_started_calculator_humanized -->
<a id='snippet-getting_started_calculator_humanized'></a>
```c#
new AppRunner<Program>()
    .UseDefaultMiddleware()
    .UseNameCasing(Case.LowerCase);
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/DocExamples/GettingStarted/Eg4_Humanized/Program.cs#L12-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_humanized' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`UseDefaultMiddleware` to take advantage of many more additional features, such as
[debug](Diagnostics/debug-directive.md) & [parse](Diagnostics/parse-directive) directives,
[ctrl+c support](OtherFeatures/cancellation.md),
[prompting](ArgumentValues/prompting.md),
[piping](ArgumentValues/piped-arguments.md),
[response files](ArgumentValues/response-files.md) and [typo suggestions](Help/typo-suggestions.md)

_see [Default Middleware](OtherFeatures/default-middleware.md) for more details and options for using default middleware._

`UseNameCasing` to enforce a consistent naming standard when generating names from your code.

_see [Name Casing](OtherFeatures/name-casing.md) for more details._

## Next Steps

You get the gist of this library now. This may be all you need to start your app.

Check out more of our documentation

* [Commands](Commands/commands.md) defining commands, subcommands and arguments.

* [Arguments](Arguments/arguments.md) defining arguments.

* [Argument Values](ArgumentValues/argument-separator.md) providing values to arguments.

* [Help](Help/help.md) options to modify help and other help features. 
 
* [Diagnostics](Diagnostics/app-version.md) a rich set of tools to simplify troubleshooting

* [Other Features](OtherFeatures/default-middleware.md) additional features available.

* [Extensibility](Extensibility/directives.md) if the framework is missing a feature you need, you can likely add it yourself. For questions, ping us on our [Discord channel](https://discord.gg/QFxKSeG) or create a [GitHub Issue](https://github.com/bilal-fazlani/commanddotnet/issues)

* [Test Tools](TestTools/overview.md) a test package to test console output with this framework. These tools enable you to provide end-to-end testing with the same experience as the console as well as testing middleware and other extensibility components. This package is used to test all of the CommandDotNet features.
