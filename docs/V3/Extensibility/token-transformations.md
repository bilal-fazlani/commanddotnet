# Token Transformations

Token transformations are a way to tranform input arguments before they are mapped to commands.

This feature is used internally to expand clubbed options and support [Response Files](../Middleware/response-files.md).

See these implementations as examples:

  * [ExpandClubbedOptionsTransformation](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Tokens/ExpandClubbedOptionsTransformation.cs)
  * [ExpandResponseFilesTransformation](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Tokens/ExpandResponseFilesTransformation.cs)

## How to implement

First, create a function with the signature `Func<CommandContext, TokenCollection, TokenCollection>`.
This function will transform a `TokenTransformation` into a new `TokenTransformation`.

Next, register the transformation with 

```c#
appRunner.Configure(c => c.UseTokenTransformation(
    name: "my-transformation", 
    order: 1, 
    transformation: MyFunc));
```

* __name__ is used for logging and the [Parse Directive](../../../directives)
* __order__ is the order the transformation should run in relation to other transformations.

## Tokens

Tokens are initially parsed from the Program.Main args array. Transformations can modify tokens or expand them into additional tokens.

Tokens contains

* __RawValue__: Raw value from the user input. This will contain the punctuation used to denote option and argument names or brackets enclosing directives. (eg. `--help`, `[debug]`)
* __Value__: Can be an Option name or an argument value or a directive(eg. `help`) 
* __TokenType__: Directive, Option, Value, Separator
* __OptionTokenType__: when TokenType is Option

OptionTokenType contains

* __IsLong__: is a long name (`--help`)
* __IsShort__: is a short name (`-h`)
* __IsClubbed__: multiple short names in a single string (`-hv`).
* __HasValue__: the option has a value assignment (`--verbosity:trace`)

## TokenCollections

TokenCollections are an immutable collection of tokens with some convenience members to simplify transformations.