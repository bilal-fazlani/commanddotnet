# Argument Collections

With CommandDotNet, you can define a single Operand collection and many Option collections.

Let's enhance our rocket launcher to visit multiple planets and specify a crew.

```c#
public void LaunchRocket(
    IEnumerable<string> planets, 
    [Option(ShortName = "c", LongName = "crew")] string[] crew)
```

Help looks like...

```bash
~
$ dotnet example.dll LaunchRocket -h
Usage: dotnet example.dll LaunchRocket [options] [arguments]

Arguments:

  planets (Multiple)  <TEXT>

Options:

  -c (Multiple)  <TEXT>
```

And this is how you call it

```bash
dotnet example.dll LaunchRocket mars earth jupiter -c aaron -c alex
```

and since options are not positional, they can be intermixed with operands.

```bash
dotnet example.dll LaunchRocket mars -c aaron earth -c alex jupiter

launching rocket
planets: mars,earch,jupiter
crew: aaron,alex
```

## Operand Collections

Only one collection is supported because it is not possible to determine which collection an operand belonged to if there were many.

Operand collections must the be the last operand specified.

## Option Collections

Since options are named, you can have multiple option collections. 

There are two ways to assign values to option collections.

The first is to specify the option multple times.

```bash
dotnet myapp.dll --name Beatrix --name Hadley 
```

The second is to define a split character for the option `[Option(Split=",")]` and then you can

```bash
dotnet myapp.dll --name Beatrix,Hadley 
```

Split can also be defined at a global level setting `AppSettings.Arguments.DefaultOptionSplit`.

In cases where the user cannot use the provided split character (perhaps the script language does not support the character), the user can override it using the `[split]` directive.  For example, if the user would prefer a hyphen, they can use

```bash
dotnet myapp.dll [split:-] --name Beatrix-Hadley 
```

## Piping

CommandDotNet provides middleware [to pipe input to operand collections](../ArgumentValues/piped-arguments.md).

If your parameter type is `IEnumerable<T>`, the operands will be streamed into the command.

## Prompts

See [prompting for missing arguments](../ArgumentValues/prompting.md#prompting-for-missing-arguments) to see how prompting for collections works.

[Replace the default prompter](../ArgumentValues/prompting.md#prompting-from-within-the-command-method) to provide a different experience.
