# Argument Separator

[Posix guideline (#10)](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html#tag_12_02) 

> The first -- argument that is not an option-argument should be accepted as a delimiter indicating the end of options. Any following arguments should be treated as operands, even if they begin with the '-' character.

This was created to handle cases where operand values begin with `-` or `--` causing the parser to interpret them as options.

Take the calculator example `add 1 2`. Let's try with negative numbers...

```bash
~
$ calculator.exe add -1 -2

Unrecognized option '-1'
```

And now with the separator

```bash
~
$ calculator.exe add -- -1 -2

3
```

With the newer [directives](directives.md) concept, `--` is also necessary for operand values encased with square brackets if those operands are the first values given.

Any unmapped operands will be stored in `CommandContext.ParseResult.RemainingOperands`.

All operands after the first `--` will be stored in `CommandContext.ParseResult.SeparatedArguments` regardless of whether they were mapped or not. This enables support for pass-thru arguments.

## Pass-thru arguments

The `--` is often used to denote arguments to be passed to a sub-process. 

For example, [dotnet.exe](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run#options) has this discription for `--`:

> Delimits arguments to dotnet run from arguments for the application being run. All arguments after this delimiter are passed to the application run.

```bash
dotnet run -- --input sunrise.CR2 --output sunrise.JPG
```

is equivalent to

```bash
imageconv.exe --input sunrise.CR2 --output sunrise.JPG
```

### How-to

Explicit support for this is complicated to provide generically because the framework cannot know

* if a commands operands are formatted like options or directives
* if a command can expects pass-thru arguments 
* if a user entered `--` to enter operand values or delimit pass-thru arguments

Due to this complexity, we'll give you the data and let you determine the best approach for your app.

Here's one approach:

Using *Add* command that takes two numbers

When `Add 1 2 -- 3 4`

* `CommandContext.ParseResult.RemainingOperands` would contain `{3, 4}`
* `CommandContext.ParseResult.SeparatedArguments` would contain `{3, 4}`

When `Add -- -1 -2 -3 -4`

* `CommandContext.ParseResult.RemainingOperands` would contain `{-3, -4}`
* `CommandContext.ParseResult.SeparatedArguments` would contain `{-1, -2, -3, -4}`

In this case, RemainingOperands always contains the pass-thru arguments.