#Prompting Middleware

Prompting is supported for two scenarios:

1. Prompting for values from within the command method.
2. Prompting for arguments where values were not provided. 

Enable prompting middleware with: `appRunner.UsePrompting(...)`

Examples can be found in the [prompts](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet.Example/Commands/Prompts.cs) 
commands in the example app.

## Prompting from within the command method

A [parameter resolver](parameter-resolvers.md) will be registered for `IPrompter`.
The IPrompter can prompt for a single value or a list. 

When prompting for a list, each entry is on a new line. Entering two empty lines will stop prompting for that value.

Use Ctrl+C to exit prompting, setting the out parameter `isCancellationRequested` to true.

Set the `isPassword` parameter to true to hide the input value.

Override the default IPrompter with the `prompterOverride` paramter 

`prompterOverride: context => new MyPrompter(context)`

## Prompting for missing arguments

`UsePrompting` enables argument prompting by default.  Use `promptForMissingArguments:false` to disable.

Prompt text can be overridden using the `argumentPromptTextOverride` parameter. This example shows using a custom attribute to provide prompt text.

``` cs
argumentPromptTextOverride: (context, argument) => $"{argument.CustomAttributes.Get<MyPromptTextAttribute>().PromptText}"
```

By default, the arguments that will be prompted are those where `argument.Arity.RequiresAtLeastOne()` and no value was provided. 
Arguments defined with nullable types or optional parameters will not be prompted.
This behavior can be changed using the `argumentFilter` parameter.

``` cs
argumentFilter: argument => argument.CustomAttributes.Get<MyPromptTextAttribute>()?.CanPrompt ?? false
```

Use the [Password](passwords.md) type to hide all characters.