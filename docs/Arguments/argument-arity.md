# Arity & Default Values

## Arity

[Arity](https://en.wikipedia.org/wiki/Arity) describes how many values a user can or must provide for an argument.

`ArgumentArity` is expressed as a _minimum_ and _maximum_ value 

* shorthand: `{min}..{max}`. eg. `0..1` min=0 max=1

When...

  * _minimum_ == 0, no values are required 
      * this applies to flags and [optional arguments](#optional-arguments)
  * _maximum_ > 1, multiple values can be provided
  * _maximum_ == int.MaxValue, an unlimited number of values can be provided

### Possible Arities

|            |none (flags)|optional|required     |
|---         |---         |---     |---          |
|single value|0..0        |0..1    |1..1         |
|list values |--          |0..N(>0)|M(>0)..N(>=M)|

* arity for single value can be
     * 0..0 `Zero`
     * 0..1 `ZeroOrOne` (optional)
     * 1..1 `ExactlyOne` (required)
* arity for list values can be
    * 0..N(>0) `ZeroOrMore` (optional) N must be greater than 0
    * M(>0)..N(>=M) `OneOrMore` (required) N must be greater than or equal to M. 
        * If N equal M, the list must have exactly M values.

Currently, the only way to set the arity for an argument to something other than one of the above is to get the instance of the argument from the `CommandContext` and assign a new `ArgumentArity` to the `IArgument.Arity`. This can be done via [middleware](../Extensibility/middleware.md) or [interceptor methods](../Extensibility/interceptors.md). We've captured [the work here](https://github.com/bilal-fazlani/commanddotnet/issues/409) but it hasn't been a priority for us. This would be an easy feature to contribute to.

### Optional Arguments 

An argument is considered optional when defined as...

* a Nullable<T> type: bool?, Guid?
* a [Nullable reference type](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/nullable-reference-types) (NRT): object?
* an optional parameter
* an `IArgumentModel` property with a default value where the default value != default(T)
    * the value must be set in the ctor or property assignment. This condition is evaluated immediately after instantiation.

### Validation

CommandDotNet will check if the minimum or maximum arity has been exceeded and raise an error.  Here's examples of how arity works.

#### Single value types

<!-- snippet: arguments_arity -->
<a id='snippet-arguments_arity'></a>
```c#
public void DefaultCommand(Model model,
        bool requiredBool, Uri requiredRefType, 
        bool? nullableBool, Uri? nullableRefType,
        bool optionalBool = false, Uri optionalRefType = null)
{}

public class Model : IArgumentModel
{
    [Operand] public bool RequiredBool { get; set; }
    [Operand] public bool DefaultBool { get; set; } = true;
    [Operand] public Uri RequiredRefType { get; set; }
    [Operand] public Uri DefaultRefType { get; set; } = new ("http://apple.com");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Arity.cs#L17-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: arguments_arity_help -->
<a id='snippet-arguments_arity_help'></a>
```bash
$ app.exe --help
Usage: app.exe <RequiredBool> <DefaultBool> <RequiredRefType> <DefaultRefType> <requiredBool> <requiredRefType> [<nullableBool> <nullableRefType> <optionalBool> <optionalRefType>]

Arguments:

  RequiredBool     <BOOLEAN>
  Allowed values: true, false

  DefaultBool      <BOOLEAN>  [True]
  Allowed values: true, false

  RequiredRefType  <URI>

  DefaultRefType   <URI>      [http://apple.com/]

  requiredBool     <BOOLEAN>
  Allowed values: true, false

  requiredRefType  <URI>

  nullableBool     <BOOLEAN>
  Allowed values: true, false

  nullableRefType  <URI>

  optionalBool     <BOOLEAN>  [False]
  Allowed values: true, false

  optionalRefType  <URI>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_arity_help.bash#L1-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here are the errors when required arguments are missing

<!-- snippet: arguments_arity_missing_args -->
<a id='snippet-arguments_arity_missing_args'></a>
```bash
$ app.exe 
RequiredBool is required
RequiredRefType is required
requiredBool is required
requiredRefType is required
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_arity_missing_args.bash#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity_missing_args' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Notice 
    there are only 4 operands listed as missing but there are 6 operands listed as required in the usage section. This is because operands are positional so even though the DefaultBool and DafaultRefType are not required based on property definition, they are effectively required because values must be provided for them before the required operands positioned after them. Keep this in mind when designing your commands.  Always position optional operands after required operands. 

#### Collection types

We'll use options for these because we can have only one collection operand per command.

<!-- snippet: arguments_arity_collection -->
<a id='snippet-arguments_arity_collection'></a>
```c#
public void DefaultCommand(
        [Option('b')] bool[] requiredBool, [Option('u')] Uri[] requiredRefType,
        [Option] bool[]? nullableBool, [Option] Uri[]? nullableRefType,
        [Option] bool[] optionalBool = null, [Option] Uri[] optionalRefType = null)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Arity.cs#L93-L98' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity_collection' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: arguments_arity_collection_help -->
<a id='snippet-arguments_arity_collection_help'></a>
```bash
$ app.exe --help
Usage: app.exe [options]

Options:

  -b | --requiredBool (Multiple)     <BOOLEAN>
  Allowed values: true, false

  -u | --requiredRefType (Multiple)  <URI>

  --nullableBool (Multiple)          <BOOLEAN>
  Allowed values: true, false

  --nullableRefType (Multiple)       <URI>

  --optionalBool (Multiple)          <BOOLEAN>
  Allowed values: true, false

  --optionalRefType (Multiple)       <URI>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_arity_collection_help.bash#L1-L21' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity_collection_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

We receive the same errors when required arguments are missing

<!-- snippet: arguments_arity_collection_missing_args -->
<a id='snippet-arguments_arity_collection_missing_args'></a>
```bash
$ app.exe 
requiredBool is required
requiredRefType is required
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_arity_collection_missing_args.bash#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_arity_collection_missing_args' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Static Helpers

`ArgumentArity` contains the following members to encapsulate the common use cases. 

<!-- snippet: known-arities -->
<a id='snippet-known-arities'></a>
```c#
public static IArgumentArity Zero => new ArgumentArity(0, 0);
public static IArgumentArity ZeroOrOne => new ArgumentArity(0, 1);
public static IArgumentArity ExactlyOne => new ArgumentArity(1, 1);
public static IArgumentArity ZeroOrMore => new ArgumentArity(0, Unlimited);
public static IArgumentArity OneOrMore => new ArgumentArity(1, Unlimited);
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ArgumentArity.cs#L37-L43' title='Snippet source file'>snippet source</a> | <a href='#snippet-known-arities' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The static method `ArgumentArity.Default(IArgument argument)` will return one of these static values based on the type

There are several extension methods that make it easier check conditions of a given arity.

<!-- snippet: arity-extensions -->
<a id='snippet-arity-extensions'></a>
```c#
/// <summary><see cref="IArgumentArity.Maximum"/> &gt; 1</summary>
public static bool AllowsMany(this IArgumentArity arity) => arity.Maximum > 1;

/// <summary><see cref="IArgumentArity.Maximum"/> &gt;= 1</summary>
public static bool AllowsOneOrMore(this IArgumentArity arity) => arity.Maximum >= 1;

/// <summary><see cref="IArgumentArity.Minimum"/> &gt; 0</summary>
public static bool RequiresAtLeastOne(this IArgumentArity arity) => arity.Minimum > 0;

/// <summary><see cref="IArgumentArity.Minimum"/> == 1 == <see cref="IArgumentArity.Maximum"/></summary>
public static bool RequiresExactlyOne(this IArgumentArity arity) => arity.Minimum == 1 && arity.Maximum == 1;

/// <summary>
/// <see cref="IArgumentArity.Maximum"/> == 0.
/// e.g. <see cref="ArgumentArity.Zero"/>
/// </summary>
public static bool RequiresNone(this IArgumentArity arity) => arity.Maximum == 0;

/// <summary>
/// <see cref="IArgumentArity.Minimum"/> == 0.
/// e.g. <see cref="ArgumentArity.Zero"/>, <see cref="ArgumentArity.ZeroOrOne"/>, <see cref="ArgumentArity.ZeroOrMore"/>
/// </summary>
public static bool AllowsNone(this IArgumentArity arity) => arity.Minimum == 0;

/// <summary>
/// <see cref="IArgumentArity.Maximum"/> == <see cref="ArgumentArity.Unlimited"/> (<see cref="int.MaxValue"/>).
/// e.g. <see cref="ArgumentArity.ZeroOrMore"/>, <see cref="ArgumentArity.OneOrMore"/>
/// </summary>
public static bool AllowsUnlimited(this IArgumentArity arity) => arity.Maximum == ArgumentArity.Unlimited;
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ArgumentArityExtensions.cs#L5-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-arity-extensions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Default Values

Middleware can set default values for arguments. Setting default values will __not__ modify the arity.

Consider there are two categories of default values

1. Statically defined by the application developer, such as described in the [optional arguments](#optional-arguments) section. 
    * These can be inferred to mean the user is not required to enter a value. 
    * This could also be defined by an attribute or some other static mechanism.
1. Default values specified by the user to simplify use of the application. For example, pulling default values from [Environment Variables and AppSettings](../ArgumentValues/default-values-from-config.md). 
    * The application requires a value but the user does not have to enter it because it is defaulted by a middleware component.

Any middleware updating the default values from statically defined sources should also update the arity. 
Use `argument.Arity = ArgumentArity.Default(argument)` to calculate a new arity for the argument.

## Summary by definition type

|defined using|defaults|optional| 
|---          |---    |---        |
|parameter    |optional parameters | when Nullable or parameter is optional |
|property     |property value immediately after initialization | |
| -- struct   |       | when Nullable or default != default(T) | 
| -- class    |       | when default != null | 

Arguments defined as parameters have consistent behavior for struct and class types.

Arguments defined as properties have have differing behavior between struct and class types.
