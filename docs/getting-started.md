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
<!-- snippet: getting_started_1_calculator -->
<a id='snippet-getting_started_1_calculator'></a>
```c#
public class Program
{
    static int Main(string[] args) => new AppRunner<Program>().Run(args);

    public void Add(int x, int y) => Console.WriteLine(x + y);

    public void Subtract(int x, int y) => Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_1_Calculator.cs#L11-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_1_calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

That's it. You now have an applciation with two commands. Let's see about how we can call it from command line.

Assuming our application's name is `calculator.dll`, let's run this app from command line using dotnet.
First we'll check out the auto-generated help.

<!-- snippet: getting_started_1_calculator_help -->
<a id='snippet-getting_started_1_calculator_help'></a>
```bash
~
$ dotnet calculator.dll --help
Usage: dotnet calculator.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calculator.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_1_calculator_help.bash#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_1_calculator_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

From the root we can see the available commands. Instead of `--help` we could have used `-h` or `-?`. 
We'll use `-h` to get help for the _Add_ command.

<!-- snippet: getting_started_1_calculator_add_help -->
<a id='snippet-getting_started_1_calculator_add_help'></a>
```bash
~
$ dotnet calculator.dll Add -h
Usage: dotnet calculator.dll Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_1_calculator_add_help.bash#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_1_calculator_add_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Let's try it out by adding two numbers

<!-- snippet: getting_started_1_calculator_add -->
<a id='snippet-getting_started_1_calculator_add'></a>
```bash
~
$ dotnet calculator.dll Add 40 20
60
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_1_calculator_add.bash#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_1_calculator_add' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

CommandDotNet will validate if the arguments can be converted to the correct type.

<!-- snippet: getting_started_1_calculator_add_invalid -->
<a id='snippet-getting_started_1_calculator_add_invalid'></a>
```bash
~
$ dotnet calculator.dll Add a 20
'a' is not a valid Number
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_1_calculator_add_invalid.bash#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_1_calculator_add_invalid' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Note
    CommandDotNet also supports running your application as an .exe and as a dotnet tool.

## Let's improve the help

The help could be more helpful. Let's add descriptions.

<!-- snippet: getting_started_2_calculator -->
<a id='snippet-getting_started_2_calculator'></a>
```c#
[Command(Description = "Performs mathematical calculations")]
public class Program
{
    static int Main(string[] args) =>
        new AppRunner<Program>().Run(args);

    [Command("Sum", Description = "Adds two numbers")]
    public void Add(int x, int y) => Console.WriteLine(x + y);

    [Command(Description = "Subtracts two numbers")]
    public void Subtract(int x, int y) => Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_2_Help.cs#L11-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_2_calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Descriptions can also be added to the arguments and extended help can be added for commands to appear below all other help.

Notice we also changed the name of the Add command to Sum.

Let's see how the help appears now.

<!-- snippet: getting_started_2_calculator_help -->
<a id='snippet-getting_started_2_calculator_help'></a>
```bash
~
$ dotnet calculator.dll --help
Performs mathematical calculations

Usage: dotnet calculator.dll [command]

Commands:

  Subtract  Subtracts two numbers
  Sum       Adds two numbers

Use "dotnet calculator.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_2_calculator_help.bash#L1-L14' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_2_calculator_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Descriptions are not required but can be very useful depending upon the complexity of your app and the audience.

Now let's see help for the _Add_ command.

<!-- snippet: getting_started_2_calculator_add_help -->
<a id='snippet-getting_started_2_calculator_add_help'></a>
```bash
~
$ dotnet calculator.dll Sum -h
Adds two numbers

Usage: dotnet calculator.dll Sum <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting_started_2_calculator_add_help.bash#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_2_calculator_add_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here we see arguments for addition and their type.  See the [Arguments](Arguments/arguments.md) section for more options to configure arguments.

## Let's add some tests

One of the problems with writing console apps is being able to automate testing the apps.
CommandDotNet solves this with our [Test Tools](TestTools/overview.md).

We make it easy to test your app as if you're entering the commands in the console.

The first step is to get access to the AppRunner the program is using so your tests are testing the application as it is configured.

Let's extract the configuration into a public static property

<!-- snippet: getting_started_calculator_testable -->
<a id='snippet-getting_started_calculator_testable'></a>
```c#
static int Main(string[] args) => AppRunner.Run(args);

public static AppRunner AppRunner => new AppRunner<Program>();
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_3_Testing.cs#L13-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_testable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Now the tests can use `Program.AppRunner` for all tests. 
!!! Note
    You could make this a singleton if you were confident you weren't caching values that would cause errors and you weren't running the tests in parallel. Generally, this won't be the source of slow runs. CommandDotNet caches the reflection results used to generate commands so tests will only incur the reflection penalty once.

CommandDotNet supports two different test patterns:

### Standard

<!-- snippet: getting_started_calculator_add_command_tests -->
<a id='snippet-getting_started_calculator_add_command_tests'></a>
```c#
[Test]
public void Given2Numbers_Should_OutputSum()
{
    var result = Program.AppRunner.RunInMem("Add 40 20");
    result.ExitCode.Should().Be(0);
    result.Console.OutText().Should().Be("60");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_3_Testing.cs#L27-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_add_command_tests' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_3_Testing.cs#L41-L61' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_calculator_add_command_tests_bdd' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See [Test Tools](TestTools/overview.md) in the Testing help section for more details 

## Let's add some validation rules.

CommandDotNet has packages to utilize the [DataAnnotations](Arguments/data-annotations-validation.md) and [FluentValidation](Arguments/fluent-validation-for-argument-models.md) frameworks.

Let's set the support for DataAnnotations and how you can use [Argument Models](Arguments/argument-models.md) to reuse argument definitions and validations.

<!-- snippet: dataannotations_1_table -->
<a id='snippet-dataannotations_1_table'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => 
        new AppRunner<Program>()
            .UseNameCasing(Case.LowerCase)
            .UseDataAnnotationValidations();

    public Task<int> Interceptor(InterceptorExecutionDelegate next, Verbosity verbosity)
    {
        // pre-execution logic here

        return next(); // Create method is executed here

        // post-execution logic here
    }

    public void Create(IConsole console, Table table, [Option, Url] string server)
    {
        console.WriteLine($"created {table.Name} as {server}. notifying: {table.Owner}");
    }
}

public class Table : IArgumentModel
{
    [Operand, Required, MaxLength(10)]
    public string Name { get; set; }

    [Option, Required, EmailAddress]
    public string Owner { get; set; }
}

public class Verbosity : IArgumentModel, IValidatableObject
{
    [Option('s', AssignToExecutableSubcommands = true)]
    public bool Silent { get; set; }
    [Option('v', AssignToExecutableSubcommands = true)]
    public bool Verbose { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Silent && Verbose)
            yield return new ValidationResult("silent and verbose are mutually exclusive. There can be only one!");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/DataAnnotations/DataAnnotations_1_Table.cs#L13-L60' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_1_table' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There's a lot more going on here. so let's break it down. 

This app has one command `create`, defined by the `Create` command. 

* The command name and all argument names are converted to lowercase thanks to `.UseNameCasing(Case.LowerCase)`.
    * see [Name Casing](OtherFeatures/name-casing.md) for more details.
* Defins arguments via `server` parameter and `Table` argument model

This app also has an interceptor method that 

* Wraps the execution of all child commands.
    * This is one way to implement pre/post hooks for a set of commands. The other is via [Middleware](Extensibility/middleware.md) components.
* Defines arguments via `Verbosity` that can be reused across all subcommands.
    * Silent and Verbose have `AssignToExecutableSubcommands=true` which means the options will appear as options for each subcommand. If this had been false, they would appear for the parent command.
    * This is an example of how to enforce consistency for cross-cutting concerns in your application.
    * Demonstrates use of IValidatableObject for complex object validation

ArgumentModels can contain other ArgumentModels and validation will be run at all levels.

Here is the help for this command. We haven't defined any descriptions or such. Notice validation logic is not shown in the help. This is something you could add with additional middleware. We also accept feature contributions :).

<!-- snippet: dataannotations_1_table_create_help -->
<a id='snippet-dataannotations_1_table_create_help'></a>
```bash
~
$ dotnet table.dll create --help
Usage: dotnet table.dll create [options] <name>

Arguments:

  name  <TEXT>

Options:

  --owner         <TEXT>

  --server        <TEXT>

  -s | --silent

  -v | --verbose
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/DataAnnotations_1_table_create_help.bash#L1-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_1_table_create_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When the command is run with invalid arguments, you get the resulting error messages. Notice we were able to club (aka bundle) the `Silent` and `Verbose` shortnames `-sv`. 

<!-- snippet: dataannotations_1_table_create -->
<a id='snippet-dataannotations_1_table_create'></a>
```bash
~
$ dotnet hr.dll create TooLongTableName --server bossman --owner abc -sv
silent and verbose are mutually exclusive. There can be only one!
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/DataAnnotations_1_table_create.bash#L1-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations_1_table_create' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Let's calculate a sum from a stream of piped arguments

<!-- snippet: getting_started_5_pipes -->
<a id='snippet-getting_started_5_pipes'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => new AppRunner<Program>();

    public void Range(IConsole console, int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count))
        {
            console.WriteLine(i);
            if (sleep > 0)
            {
                Thread.Sleep(sleep);
            }
        }
    }

    public void Sum(IConsole console, IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values)
        {
            console.WriteLine(total += value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_5_Pipes.cs#L12-L40' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_5_pipes' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here we've converted the arguments for Sum into an IEnumerable<int> and added a Range command.
You've probably noticed these commands wrap LINQ methods of the same name. 
We've added an optional sleep option to Range to better mimic a long running stream. 

We could have used List<int>, int[], or any other collection type. 
Using IEnumerable<T> allows the command to start processing before the stream has completed.

Very few console frameworks make it this easy to write streaming console tools.

Let's see it in action:

```bash
~
$ dotnet linq.dll Range 1 4 10000 | dotnet linq.dll Sum
1
3
6
10
```

After outputtting a value, Range sleeps for 10 seconds.  We know Sum is streaming because it immediatly outputs the new sum as soon as it receives a value and waits for the next value.

## Let's handle Ctrl+C

The above command will take 40 seconds to execute. The way it's currently configured, we have no way to exit early.

With console applications, the standard pattern is to exit the app when Ctrl+C is pressed.  Here's how we support that pattern with CommandDotNet.

<!-- snippet: getting_started_6_ctrlc -->
<a id='snippet-getting_started_6_ctrlc'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => 
        new AppRunner<Program>()
            .UseCancellationHandlers();

    public void Range(IConsole console, CancellationToken ct, int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count).UntilCancelled(ct, sleep))
        {
            console.WriteLine(i);
        }
    }

    public void Sum(IConsole console, CancellationToken ct, IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values.ThrowIfCancelled(ct))
        {
            console.WriteLine(total += value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_6_CtrlC.cs#L11-L37' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_6_ctrlc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Again, CommandDotNet makes this very easy. Configure the app with `UseCancellationHandlers()` and a `CancellationToken` can be injected into your commands. 

Use either of the two handy extension methods `UntilCancelled` or `ThrowIfCancelled` to exit an enumeration early when cancellation has been requested.

## Opt-In to additional features

In the `Program.Main`, we configured the app with the basic feature set.

<!-- snippet: getting_started_other_features -->
<a id='snippet-getting_started_other_features'></a>
```c#
new AppRunner<Program>()
    .UseDefaultMiddleware();
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_OtherFeatures.cs#L10-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_other_features' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`UseDefaultMiddleware` to take advantage of many more additional features, such as
[debug](Diagnostics/debug-directive.md) & [parse](Diagnostics/parse-directive) directives,
[ctrl+c support](OtherFeatures/cancellation.md),
[prompting](ArgumentValues/prompting.md),
[piping](ArgumentValues/piped-arguments.md),
[response files](ArgumentValues/response-files.md) and [typo suggestions](Help/typo-suggestions.md)

_see [Default Middleware](OtherFeatures/default-middleware.md) for more details and options for using default middleware._

## Next Steps

You get the gist of this library now. This may be all you need to start your app.  If not, we've only touched on a small number of our features. Check out our documentation for more...

* [Commands](Commands/commands.md) defining commands, subcommands and arguments.

* [Arguments](Arguments/arguments.md) defining arguments.

* [Argument Values](ArgumentValues/argument-separator.md) providing values to arguments.

* [Help](Help/help.md) options to modify help and other help features. 
 
* [Diagnostics](Diagnostics/app-version.md) a rich set of tools to simplify troubleshooting

* [Other Features](OtherFeatures/default-middleware.md) additional features available.

* [Extensibility](Extensibility/directives.md) if the framework is missing a feature you need, you can likely add it yourself. For questions, ping us on our [Discord channel](https://discord.gg/QFxKSeG) or create a [GitHub Issue](https://github.com/bilal-fazlani/commanddotnet/issues)

* [Test Tools](TestTools/overview.md) a test package to test console output with this framework. These tools enable you to provide end-to-end testing with the same experience as the console as well as testing middleware and other extensibility components. This package is used to test all of the CommandDotNet features.
