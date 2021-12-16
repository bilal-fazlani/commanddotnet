# Improving the Help

The help you've seen so far is what can be inferred from the code. 
Let's make it more useful by adding descriptions, usage examples and extended help text.

<!-- snippet: getting-started-300-calculator -->
<a id='snippet-getting-started-300-calculator'></a>
```c#
[Command(
    Description = "Performs mathematical calculations",
    ExtendedHelpTextLines = new []
    {
        "Include multiple lines of text",
        "Extended help of the root command is a good place to describe directives for the app"
    })]
public class Program
{
    static int Main(string[] args) =>
        new AppRunner<Program>().Run(args);

    [Command(
        Description = "Adds two numbers",
        UsageLines = new []
        {
            "Add 1 2",
            "%AppName% %CmdPath% 1 2"
        },
        ExtendedHelpText = "single line of extended help here")]
    public void Add(
        [Operand(Description = "first value")] int x,
        [Operand(Description = "second value")] int y) => Console.WriteLine(x + y);

    [Command(Description = "Subtracts two numbers")]
    public void Subtract(int x, int y) => Console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_300_Help.cs#L12-L40' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-300-calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

For commands, there are three elements we can provide to enhance help; Description, Usage and ExtendedHelpText.

For arguments we have Description.

For cases where you want to output multiple lines, there's a corresponding DescriptionLines, UsageLines and ExtendedHelpTextLines. 
These will ensure the newline characters match the OS, simplifying cross-platform testing.

Let's see how the help appears now.

<!-- snippet: getting-started-300-calculator-help -->
<a id='snippet-getting-started-300-calculator-help'></a>
```bash
$ dotnet calculator.dll --help
Performs mathematical calculations

Usage: dotnet calculator.dll [command]

Commands:

  Add       Adds two numbers
  Subtract  Subtracts two numbers

Use "dotnet calculator.dll [command] --help" for more information about a command.

Include multiple lines of text
Extended help of the root command is a good place to describe directives for the app
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-300-calculator-help.bash#L1-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-300-calculator-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the description printed at the top and the extended help printed at the bottom.

Now let's see help for the _Add_ command.

<!-- snippet: getting-started-300-calculator-add-help -->
<a id='snippet-getting-started-300-calculator-add-help'></a>
```bash
$ dotnet calculator.dll Add -h
Adds two numbers

Usage: Add 1 2
dotnet calculator.dll Add 1 2

Arguments:

  x  <NUMBER>
  first value

  y  <NUMBER>
  second value

single line of extended help here
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-300-calculator-add-help.bash#L1-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-300-calculator-add-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice there are multiple lines of text in the Usage example. 

Also notice we used a template in the second line to include the %AppName% `dotnet calculator.dll` and %CmdPath% `Add`. %CmdPath% will print all parent commands for the current command so you do not have to worry about changing the hierarchy of the commands invalidating the Usage example.

A more terse version of help for arguments can be used by setting `AppSettings.Help.TextStyle = HelpTextStyle.Basic`.

<!-- snippet: getting-started-300-calculator-add-basic-help -->
<a id='snippet-getting-started-300-calculator-add-basic-help'></a>
```bash
$ dotnet calculator.dll Add -h
Adds two numbers

Usage: Add 1 2
dotnet calculator.dll Add 1 2

Arguments:
  x  first value
  y  second value

single line of extended help here
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-300-calculator-add-basic-help.bash#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-300-calculator-add-basic-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See the [Commands](../Commands/commands.md) section for more options to configure arguments.

See the [Arguments](../Arguments/arguments.md) section for more options to configure arguments.

Read more about [HelpTextStyle here](../Help/help.md#textstyle). 

You can override the default [HelpTextProvider](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Help/HelpTextProvider.cs).  The [BasicHelpTextProvider](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Help/BasicHelpTextProvider.cs) is an example of how to do it. You could change the entire structure, add color, etc. If you improve it, consider contributing the improvement back to the project another provider, or create a package and let us know about it.

