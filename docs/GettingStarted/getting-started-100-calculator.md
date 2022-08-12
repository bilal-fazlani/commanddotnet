# Your first console app

Let's create a calculator console application with Add & Subtract operations.

Your first step is to create a console application.

Begin by creating the commands:
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

That's it. You now have an application with two commands. Let's see about how we can call it from command line.

Assuming our application's name is `calculator.dll`, let's run this app from command line using dotnet.
First we'll check out the auto-generated help.

<!-- snippet: getting-started-100-calculator-help -->
<a id='snippet-getting-started-100-calculator-help'></a>
```bash
$ dotnet calculator.dll --help
Usage: dotnet calculator.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calculator.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-100-calculator-help.bash#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-100-calculator-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

From the root we can see the available commands. Instead of `--help` we could have used `-h` or `-?`. 
We'll use `-h` to get help for the _Add_ command.

<!-- snippet: getting-started-100-calculator-add-help -->
<a id='snippet-getting-started-100-calculator-add-help'></a>
```bash
$ dotnet calculator.dll Add -h
Usage: dotnet calculator.dll Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-100-calculator-add-help.bash#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-100-calculator-add-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Let's try it out by adding two numbers

<!-- snippet: getting-started-100-calculator-add -->
<a id='snippet-getting-started-100-calculator-add'></a>
```bash
$ dotnet calculator.dll Add 40 20
60
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-100-calculator-add.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-100-calculator-add' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

CommandDotNet will validate if the arguments can be converted to the correct type.

<!-- snippet: getting-started-100-calculator-add-invalid -->
<a id='snippet-getting-started-100-calculator-add-invalid'></a>
```bash
$ dotnet calculator.dll Add a 20
'a' is not a valid Number
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-100-calculator-add-invalid.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-100-calculator-add-invalid' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Note
    Usage can be updated to support .exe or when run as a dotnet tool. See the [UsageAppNameStyle & UsageAppName](../Help/help.md#usageappnamestyle) sections.
