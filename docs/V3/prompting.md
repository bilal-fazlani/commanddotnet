#Prompting Middleware

Enable prompting middleware with: `appRunner.UsePrompting(...)`

Examples can be found in the [prompts](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet.Example/Commands/Prompts.cs) 
commands in the example app.

## IPrompter

This will register a [parameter resolver](parameter-resolvers.md) for `IPrompter`.
The IPrompter can prompt for a single value or a list of values. 

When prompting for a list of values, each entry is on a new line. Entering two empty lines will stop prompting for that value.

Use Ctrl+C to exit prompting, setting the out parameter `isCancellationRequested` to true.

Set the `isPassword` parameter to true to hide the input value.

Override the default IPrompter with the `prompterOverride` paramter 

`prompterOverride: context => new MyPrompter(context)`

## Argument Prompting

`UsePrompting` enables argument prompting by default.  Use `promptForMissingArguments:false` to disable.

Prompt text can be overridden using the `argumentPromptTextOverride` parameter.  

``` cs
argumentPromptTextOverride: (context, argument) => $"{argument.CustomAttributes.Get<MyPromptTextAttribute>().PromptText}"
```

By default, the arguments that will be prompted are those where `argument.Arity.RequiresAtLeastOne()` and no value was provided. 
Arguments defined with nullable types or optional parameters will not be prompted.
This behavior can be changed using the `argumentFilter` parameter.

``` cs
argumentFilter: argument => argument.CustomAttributes.Get<MyPromptTextAttribute>()?.Prompt ?? false
```

Use the [Password](passwords.md) type hide the input.