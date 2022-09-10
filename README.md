<img src="./images/logo.png" width="250px" />

![Nuget](https://img.shields.io/nuget/v/commanddotnet?style=for-the-badge)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![NuGet](https://img.shields.io/nuget/dt/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![GitHub](https://img.shields.io/github/license/bilal-fazlani/commanddotnet?style=for-the-badge)](https://github.com/bilal-fazlani/commanddotnet/blob/master/LICENSE)

[![GitHub last commit](https://img.shields.io/github/last-commit/bilal-fazlani/CommandDotNet.svg?style=for-the-badge)]()
![Netlify](https://img.shields.io/netlify/ce6331f7-bbfb-4a8a-ba7c-705b2902c4f5?label=Netlify%20Build&style=for-the-badge)
[![Build](https://img.shields.io/github/workflow/status/bilal-fazlani/commanddotnet/Test/master?style=for-the-badge)](https://github.com/bilal-fazlani/commanddotnet/actions/workflows/test.yml)

[![Gitter](https://img.shields.io/gitter/room/badges/shields.svg?style=for-the-badge)](https://gitter.im/CommandDotNet/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Discord](https://img.shields.io/discord/678568687556493322?label=Discord%20Chat&style=for-the-badge)](https://discord.gg/QFxKSeG)

# CommandDotNet

### A modern framework for building modern CLI apps

Out of the box support for commands, sub-commands, validations, dependency injection, 
piping and streaming, enums & custom types, typo suggestions, prompting, passwords, response files and much more! 
See the [features page](https://commanddotnet.bilal-fazlani.com/features). 

Favors [POSIX conventions](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html)

Includes [test tools](TestTools/overview.md) used by the framework to test all features of the framework.

Modify and extend the functionality of the framework through configuration and middleware.

### Documentation 👉 https://commanddotnet.bilal-fazlani.com

## Project Status

This project is stable. Lack of new features are a result of the maturity of the library, not an indication of the liveliness of this project. 
We are available to fix bugs and answer questions and remain fairly responsive. There are PRs and issues in the backlog for new features that are currently low priority for the maintainers.  We will accept PRs.  If you haven't discussed them with us first and the change is significant, please consider the submission the beginning of a design discussion.

## Support

For bugs, [create an issue](https://github.com/bilal-fazlani/commanddotnet/issues/new)

For questions and feature requests, start [a discussion](https://github.com/bilal-fazlani/commanddotnet/discussions)

For a quick walkthrough of features, see our [Getting Started guides](https://commanddotnet.bilal-fazlani.com/gettingstarted/getting-started-0/)

Here's a starter:

### Example
Begin by creating the commands:

<!-- snippet: getting-started-100-calculator -->
<a id='snippet-getting-started-100-calculator'></a>
```cs
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

Check out the docs for more examples

See our [Getting Started guides](https://commanddotnet.bilal-fazlani.com/gettingstarted/getting-started-0/) to see how to improve the help documentation, test the application and utilize the many other features of CommandDotNet.

## Credits 🎉

Special thanks to [Drew Burlingame](https://github.com/drewburlingame) for continuous support and contributions
