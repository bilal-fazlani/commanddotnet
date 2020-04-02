# Separated Arguments

Separated arguments appear after a `--`. For example `myapp.exe Add 1 2 -- some separated args`.

These arguments will be captured `CommandContext.ParseResult.SeparatedArguments` as `IReadOnlyCollection<string>`.

To access them in a [command](commands.md) or [interceptor](interceptors.md) method, define a `string[]` parameter and attribute with `SeparatedArgumentsAttribute`.

```c#
public void Add(int x, int y, [SeparatedArguments] string[] separatedArgs)
{
    ...
}
```

The SeparatedArgumentsAttribute can be used in [interceptor](interceptors.md) methods and properties of [argument models](argument-models).md

## Appearance in Help

The usage section for a command will be postfixed with ` [[--] <arg>...]` when

* the command is executable
* the command or any interceptor method, including inherited inteceptors, has a SeparatedArgumentsAttribute attributed member. 

`Usage: myapp.exe Add [arguments] [[--] <arg>...]`

## Remaining Arguments

A similar concept is remaining arguments. When `AppSettings.IgnoreUnexpectedOperands` is set to true, remaining arguments are arguments that were not mapped to any defined Options or Operands. They can be accessed via `ComandContext.ParseResult.RemainingArguments` or by adding an operand collection to a command.

!!! Note
    `ComandContext.ParseResult.RemainingArguments` will always be empty when a command has an operand collection.