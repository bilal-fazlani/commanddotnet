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

### Optional Arguments 

An argument is considered optional when defined as...

* a Nullable<T> type: bool?, Guid?
* a [Nullable reference type](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/nullable-reference-types) (NRT): object?
* an optional parameter
* an `IArgumentModel` property with a default value where the default value != default(T)
    * the value must be set in the ctor or property assignment. This condition is evaluated immediately after instantiation.

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
