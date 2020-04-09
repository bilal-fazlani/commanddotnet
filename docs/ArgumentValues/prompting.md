# Prompting

## TLDR, How to enable 
Enable the feature with `appRunner.UsePrompting()` or `appRunner.UseDefaultMiddleware()`.

## Introduction

Prompting is supported for two scenarios:

1. Prompting for values from within the command method.
2. Prompting for arguments where values were not provided. 

Examples can be found in the [prompts](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Commands/Prompts.cs) 
commands in the example app.

## Prompting from within the command method

A [parameter resolver](../Extensibility/parameter-resolvers.md) will be registered for `IPrompter`.
The IPrompter can prompt for a single value or a list. 

```c#
[Command(Description = "knock-knock joke, demonstrating use of IPrompter")]
public void Knock(IConsole console, IPrompter prompter)
{
    if (prompter.TryPromptForValue("who's there?", out var answer1, out bool isCancellationRequested) && !isCancellationRequested)
    {
        var answer2 = prompter.PromptForValue($"{answer1} who?", out isCancellationRequested);
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

Override the default IPrompter with the `prompterOverride` paramter 

`prompterOverride: context => new MyPrompter(context)`

## Prompting for missing arguments

`UsePrompting` enables argument prompting by default.  Use `promptForMissingArguments:false` to disable.

Prompt text can be overridden using the `argumentPromptTextOverride` parameter. This example shows using a custom attribute to provide prompt text.

``` cs
argumentPromptTextOverride: (context, argument) => $"{argument.CustomAttributes.Get<MyPromptAttribute>().PromptText}"
```

By default, the arguments that will be prompted are those where `argument.Arity.RequiresAtLeastOne()` and no value was provided. 
Arguments defined with nullable types or optional parameters will not be prompted.
This behavior can be changed using the `argumentFilter` parameter.

``` cs
argumentFilter: argument => argument.CustomAttributes.Get<MyPromptAttribute>() != null
```

## Passwords

Use the [Password](../Arguments/passwords.md) type to hide all characters for an argument.

```cs
public void Login(string username, Password password){...}
```