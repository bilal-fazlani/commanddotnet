# Spectre

Use [Spectre AnsiConsole](https://spectreconsole.net/) with CommandDotNet for richer interactive experience for your applications. 

Spectre.Console.AnsiConsole Features (from their site)

* Easily output text with different colors and even styles such as bold, italic and blinking with a Rich inspired markup language.
* Supports 3/4/8/24-bit colors in the terminal with auto-detection of the current terminal's capabilities.
* Render complex widgets such as tables, trees, and even ASCII images.
* Display progress for long running tasks with live displays of progress and status controls.
* Prompt user input with strongly typed text input or via single-item select and multiple item select controls.
* Format .NET exceptions with custom color coded themes and styles.
* Written with unit testing in mind.

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.Spectre
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.Spectre
    ```
Enable the features with `appRunner.UseSpectreAnsiConsole(...)` and / or `appRunner.UseSpectreArgumentPrompter(...)`

## IAnsiConsole

`UseSpectreAnsiConsole(...)`

Makes `IAnsiConsole` available as a paramter for the commands. When used with CommandDotNet.TestTools or CommandDotNet.Spectre.Testing package, the output can be captured for unit tests. The instance of `IAnsiConsole` injected also implements `IConsole` so there's no need to update any existing code.

IAnsiConsole only outputs to the Console.Out stream. Continue to use `IConsole.Error` to output to the Console.Error stream.

```c#
public class Calculator
{
    public void Sum(IAnsiConsole console, int x, int y)
    {
        console.Markup($"The sum of [bold yellow]x[/] and [bold yellow]y[/] is [red]{x + y}[/]");
    }
}
```

To capture IAnsiConsole specific outputs in tests or to set prompt expectations, you'll need to override the default console.

See examples of [our tests in the repo](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.Tests/CommandDotNet.Spectre) and [these tests](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.Tests/CommandDotNet.Spectre/SpectreArgumentPrompterTests.cs) specifically for prompting.

```
    [Test]
    public void TestPrompts()
    {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("lala");

            new AppRunner<KnockKnock>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                ....
    }
```

## Prompt for missing arguments

`appRunner.UseSpectreArgumentPrompter(...)`

This one is exciting. By default, when an argument is not specified and it's [arity](../Arguments/argument-arity.md) requires at least one, CommandDotNet will prompt the user for the argument using one of Spectre's prompt features. If the argument is an enum, or the AllowedValues property is populated via middleware, the [selection](https://spectreconsole.net/prompts/selection) or [multi-selection](https://spectreconsole.net/prompts/multiselection) prompt will be used, allowing the user to scroll and select a specific value.  Arguments of type `Password` are kept secret.
