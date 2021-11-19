# Prompting

## TLDR, How to enable
Enable the feature with `appRunner.UsePrompter()` and / or `appRunner.UseArgumentPrompter()`.

!!! Tip
    Check out the [Spectre extensions](../OtherFeatures/spectre.md) for a richer prompting experience.
    
    Use the `AnsiTestConsole` from the `CommandDotNet.Spectre.Testing`package to test the IAnsiConsole features work as expected.
    Example included in the link above.

## Introduction

Prompting is supported for two scenarios:

1. Prompting for values from within the command method.
2. Prompting for arguments where values were not provided.

Examples can be found in the [prompts](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Commands/Prompts.cs) commands in the example app.

## Prompting from within the command method

When using `appRunner.UsePrompter()`, a [parameter resolver](../Extensibility/parameter-resolvers.md) will be registered for `IPrompter`.
The IPrompter can prompt for a single value or a list.

```c#
[Command(Description = "knock-knock joke, demonstrating use of IPrompter")]
public void Knock(IConsole console, IPrompter prompter)
{
    if (prompter.TryPromptForValue("who's there?", out var answer1, out _))
    {
        var answer2 = prompter.PromptForValue(
                $"{answer1} who?", out isCancellationRequested);
        if(!isCancellationRequested)
        {
            console.Out.WriteLine($"{answer2}");
            console.Out.WriteLine("lulz");
        }
    }
}
```

When prompting for a list, each entry is on a new line. Entering two empty lines will stop prompting for that value.

Use Ctrl+C to exit prompting, setting the out parameter `isCancellationRequested` to true.

Set the `isPassword` parameter to true to hide the input value.

Override the default IPrompter with the `prompterFactory` paramter...

`prompterFactory: context => new MyPrompter(context)`

### Handling Ctrl+C

Users may decide to cancel the operation mid-prompt. It's easy to forget this use-case when prompting for information. We've accounted for it by including the `out bool isCancellationRequested` parameter which will be true if a user entered Ctrl+C.

If you'd prefer to ignore it, just discard it like this: `prompter.PromptForValue("knock knock", out _);`

## Prompting for missing arguments

`appRunner.UseArgumentPrompter()` enables prompting for arguments where the ArgumentArity requires at least 1 value but none were provided.

Prompt text can be overridden using the `argumentPrompterFactory` parameter to construct a new ArgumentPrompter and providing a `getPromptTextCallback`. 

This example shows how you could create a custom attribute to provide prompt text.

``` cs
argumentPrompterFactory: (context, prompter) => new ArgumentPrompter(prompter, (ctx, argument) =>  
    $"{argument.CustomAttributes.Get<MyPromptAttribute>().PromptText}")
```

By default, the arguments that will be prompted are those where `argument.Arity.RequiresAtLeastOne()` and no value was provided.
Arguments defined with nullable types or optional parameters will not be prompted.
This behavior can be changed using the `argumentFilter` parameter.

``` cs
argumentFilter: argument => 
    argument.CustomAttributes.Get<MyPromptAttribute>() != null 
    && argument.Arity.RequiresAtLeastOne() 
    && !argument.HasValueFromInputOrDefault()
```

## Passwords

Use the [Password](../Arguments/passwords.md) type to hide all characters for an argument.

```c#
public void Login(string username, Password password){...}
```
