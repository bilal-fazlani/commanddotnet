# Commands

Commands are defined by methods and classes.

Using our calculator example...

<!-- snippet: getting-started-100-calculator -->
<a id='snippet-getting-started-100-calculator'></a>
```c#
public class Program
{
    // this is the entry point of your application
    static int Main(string[] args)
    {
        // AppRunner<T> where T is the class defining your commands
        // You can use Program or create commands in another class
        return new AppRunner<Program>().Run(args);
    }

    // Add command with two positional arguments
    public void Add(int x, int y) => Console.WriteLine(x + y);

    // Subtract command with two positional arguments
    public void Subtract(int x, int y) => Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_100_Calculator.cs#L11-L28' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-100-calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

__Program__ is the root command and is not directly referenced in the terminal. The root command is the type specified in `AppRunner<TRootCommand>`

__Add__ and __Subtract__ are the commands. 

Command methods must:

- be `public`
- return `void`, `int`, `Task` or `Task<int>`
    - when the return type is `int` or `Task<int>` the value is used as the exit code.

Command methods may be async.

Only public methods defined within the class will be commands. Methods from base classes are not included. 
Set `AppSettings.Commands.InheritCommandsFromBaseClasses = true` to include public methods from base classes. 
Methods from `System.Object` and `IDisposable` are not included.

## Command Attribute

Every public method will be interpreted as a command and the command name will be the method name.

Use the `[Command]` attribute to change the command name, enhance help output and provide parser hints.

<!-- snippet: commands_calculator -->
<a id='snippet-commands_calculator'></a>
```c#
[Command("Sum",
    Usage = "sum <int> [<int> ...]",
    Description = "sums all the numbers provided",
    ExtendedHelpText = "more details and examples could be provided here")]
public void Add(IConsole console, IEnumerable<int> numbers) =>
    console.WriteLine(numbers.Sum());
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Commands/Commands/Commands_Calculator.cs#L13-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-commands_calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: commands_calculator_sum_help -->
<a id='snippet-commands_calculator_sum_help'></a>
```bash
$ dotnet calculator.dll Sum --help
sums all the numbers provided

Usage: sum <int> [<int> ...]

Arguments:

  numbers (Multiple)  <NUMBER>

more details and examples could be provided here
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/commands_calculator_sum_help.bash#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-commands_calculator_sum_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Note
    Use of `[Command]` attribute is optional and only required when you need to add customizations. 

Use `IgnoreUnexpectedOperands` & `ArgumentSeparatorStrategy` to override argument parsing behavior for the command. See [Argument Separator](../ArgumentValues/argument-separator.md) for more details.

Use `Usage` to override the auto-generated usage section.

Use `Description` & `ExtendedHelpText` to include additional information in help output.

`ExtendedHelpText` on the root command is a good place to mention additional features, such as any directives the user could take advantage of

<!-- snippet: extended_help_text -->
<a id='snippet-extended_help_text'></a>
```c#
ExtendedHelpText = "Directives:\n" +
                   "  [debug] to attach a debugger to the app\n" +
                   "  [parse] to output how the inputs were tokenized\n" +
                   "  [log] to output framework logs to the console\n" +
                   "  [log:debug|info|warn|error|fatal] to output framework logs for the given level or above\n" +
                   "\n" +
                   "directives must be specified before any commands and arguments.\n" +
                   "\n" +
                   "Example: %AppName% [debug] [parse] [log:info] math")]
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Examples.cs#L7-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-extended_help_text' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Note
    If you're looking to change the app name, set `AppSettings.Help.UsageAppName` and use the `%AppName%` template variable mentioned below. Learn more in [Help](../Help/help.md#usageappnamestyle)

### Template variables

Two template variables are available for use in Usage, Description and ExtendedHelpText: `%AppName%` and `%CmdPath%`

<!-- snippet: commands_git -->
<a id='snippet-commands_git'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);
    public static AppRunner AppRunner => new AppRunner<Program>().UseNameCasing(Case.KebabCase);

    [Subcommand]
    public class Stash
    {
        [DefaultCommand]
        public void StashImpl(IConsole console) => console.WriteLine("stash");

        [Command(Usage = "%AppName% %CmdPath%")]
        public void Pop(IConsole console) => console.WriteLine("pop");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Commands/Commands/Commands_Git.cs#L9-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-commands_git' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

help for Pop may have `Usage: git Stash Pop`

- %AppName% = `git`
- %CmdPath% = `Git Stash Pop`

This is useful when you want to override the Usage example and still want to include how the command would be called.

#### AppName

Use `%AppName%` to include the name as calculated by CommandDotNet. This will use `AppSettings.Help.UsageAppName` if it's set. 

#### CmdPath

Use `%CmdPath%` to include the full path of commands. This is helpful when working with subcommands.

## Default Command

Let's assume we have a program with a single command defined, called `Process`. 

<!-- snippet: commands_default_command -->
<a id='snippet-commands_default_command'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);
    public static AppRunner AppRunner => new AppRunner<Program>();

    [DefaultCommand]
    public void Process()
    {
        // do very important stuff
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Commands/Commands/Commands_DefaultCommand.cs#L5-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-commands_default_command' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Because the Program is considered the RootCommand, you'll need to explicitely call the Process command. eg.

```bash
dotnet myapp.dll Process
```

If the root command has only one command, you may want to exectute it by default without specifying any commands names. 
We can do this with the `[DefaultCommand]` attribute.

Now executed as

```bash
dotnet myapp.dll
```

Default commands are also useful in cases like `Git Stash` above where a command can have subcommands, but can execute an action when no subcommand is specified.
