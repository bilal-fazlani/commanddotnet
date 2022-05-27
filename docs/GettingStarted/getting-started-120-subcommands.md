# Subcommands

Subcommands are any commands the user will specify when calling your application.

In our calculator app, Add and Subtract are both subcommands of the calculator app.

When an application has a lot of commands, it can be useful to group them under other commands.

For example, Git has several commands under `git stash` and dotnet has several commands under `dotnet nuget` and `dotnet tool`.

CommandDotNet supports grouping commands using classes and nested classes.

Let's enhance our calculator by adding additional Trigonometry operations and common algorithms.

<!-- snippet: getting-started-120-subcommands -->
<a id='snippet-getting-started-120-subcommands'></a>
```c#
public class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Program>().Run(args);
    }

    public void Add(int x, int y) => Console.WriteLine(x + y);

    public void Subtract(int x, int y) => Console.WriteLine(x - y);
    
    [Subcommand]
    // The Command attribute is NOT required.
    // The command name will default from the class name.
    // Use the Command attribute to change the name of the command,
    //   eg from Trigonometry to Trig
    [Command("Trig")]
    public class Trigonometry
    {
        public void Sine(int x) => Console.WriteLine(Math.Sin(x));

        public void Cosine(int x) => Console.WriteLine(Math.Cos(x));
    }

    // The command name will default form the class name.
    // Use RenameAs to change the name of the command.
    //   eg. from Algorithms to Algo
    [Subcommand(RenameAs = "Algo")]
    public Algorithms Algorithms { get; set; }
}

public class Algorithms
{
    public void Fibonacci(int depth = 10)
    {
        int a = 0, b = 1, c;
        Console.WriteLine(a);
        for (int i = 2; i < depth; i++)
        {
            c = a + b;
            Console.WriteLine(c);
            a = b;
            b = c;
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_120_Subcommands.cs#L12-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-120-subcommands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the use of `[Subcommand]` on either a nested class (Trigonometry) or a property of the type of the class (Algorithms). 

`[Subcommand]` is required to configure a class as a subcommand, `[Command]` is *NOT*.

Command names default from the class name and can be changed by using `[Command("...")]` or `[Subcommand(RenameAs="...")]`

Here is the help for the calculator showing the new Algo and Trig commands.

<!-- snippet: getting-started-120-subcommands-help -->
<a id='snippet-getting-started-120-subcommands-help'></a>
```bash
$ dotnet calculator.dll --help
Usage: dotnet calculator.dll [command]

Commands:

  Add
  Algo
  Subtract
  Trig

Use "dotnet calculator.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-120-subcommands-help.bash#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-120-subcommands-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And here is the help for the Trig command to show the Sine and Cosine commands within.

<!-- snippet: getting-started-120-subcommands-trig-help -->
<a id='snippet-getting-started-120-subcommands-trig-help'></a>
```bash
$ dotnet calculator.dll Trig --help
Usage: dotnet calculator.dll Trig [command]

Commands:

  Cosine
  Sine

Use "dotnet calculator.dll Trig [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-120-subcommands-trig-help.bash#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-120-subcommands-trig-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the Trig command is included in the usage examples.

## Summary

Commands are defined by both classe and methods. Classes can be used to group a set of related commands. Next, see how we can define a default command for a class.
