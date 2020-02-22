# Argument Arity

[Arity](https://en.wikipedia.org/wiki/Arity) indicates the number of values an argument can contain. For example, 

* Enumerable arguments can have zero or more values
* Flags cannot have any values
* All others can have zero or one value
* Required arguments must have at least one value
  * Required==true when there's no default value and the type is not nullable.

!!! warn
    `IArgumentModel` properties for reference types cannot be inferred to have a `Min` of 0.

`ArgumentArity` is defined by a `min` and `max` value.  When `min` is 0, no values are required.  When `max` is greater than 1, multiple values can be provided. 

!!! note
    There is currently no validation or way to set the arity via attributes. 
    It is left to developers to define middleware to modify and validate the arity.

Arity can be specified when creating arguments, but we recommend using the `ArgumentArity.Default` method shown below.

```c#
public static IArgumentArity Default(Type type, bool hasDefaultValue, BooleanMode booleanMode)
{
    if (type == typeof(bool) && booleanMode == BooleanMode.Unknown)
    {
        throw new ArgumentException($"{nameof(booleanMode)} cannot be {nameof(BooleanMode.Unknown)}");
    }

    bool isRequired = !(hasDefaultValue || type.IsNullableType());

    if (type != typeof(string) && type.IsEnumerable())
    {
        return isRequired ? OneOrMore : ZeroOrMore;
    }

    if (booleanMode == BooleanMode.Implicit && (type == typeof(bool) || type == typeof(bool?)))
    {
        return Zero;
    }

    return isRequired ? ExactlyOne : ZeroOrOne;
}
```
