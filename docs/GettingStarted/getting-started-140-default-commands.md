# Default Commands

As we saw in the previous examples, a class can define commands using methods. 

In those examples, the commands defined as methods are always subcommands.

There may be a case where an application should not have a subcommand 

Or a case where a subcommand should have a default behavior and contain subcommands, such as Git's `stash` command, where `stash` will stash changes and `stash pop` will pop them from the stash stack.

CommandDotNet supports this with the `[DefaultCommand]` attribute used to decorate the method to execute.

<!-- snippet: getting-started-140-default-commands -->
<a id='snippet-getting-started-140-default-commands'></a>
```c#
public class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Program>().Run(args);
    }

    [DefaultCommand]
    public void Execute(int x, int y) => Console.WriteLine(x + y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_140_Default_Commands.cs#L13-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-140-default-commands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: getting-started-140-default-commands-help -->
<a id='snippet-getting-started-140-default-commands-help'></a>
```bash
$ add.exe Add -h
Usage: add.exe <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-140-default-commands-help.bash#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-140-default-commands-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: getting-started-140-default-commands-add -->
<a id='snippet-getting-started-140-default-commands-add'></a>
```bash
$ add.exe 40 20
60
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-140-default-commands-add.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-140-default-commands-add' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Command per class pattern

Many console frameworks are designed solely to support the pattern of defining a command with a class.
With these frameowrks, the command class will contain an Execute method.

CommandDotNet does not force this pattern but can support it. Just create a class and decorate a method with `[DefaultCommand]`


<!-- snippet: getting-started-140-default-commands-command-per-class -->
<a id='snippet-getting-started-140-default-commands-command-per-class'></a>
```c#
public class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Program>().Run(args);
    }

    [Subcommand]
    public Add Add { get; set; }

    [Subcommand]
    public Subtract Subtract { get; set; }
}

public class Add
{
    [DefaultCommand]
    public void Execute(int x, int y) => Console.WriteLine(x + y);
}

public class Subtract
{
    [DefaultCommand]
    public void Execute(int x, int y) => Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_140_Default_Commands.cs#L28-L54' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-140-default-commands-command-per-class' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: getting-started-140-default-commands-help-command-per-class -->
<a id='snippet-getting-started-140-default-commands-help-command-per-class'></a>
```bash
$ dotnet calculator.dll --help
Usage: dotnet calculator.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calculator.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-140-default-commands-help-command-per-class.bash#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-140-default-commands-help-command-per-class' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
