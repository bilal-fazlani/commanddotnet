# Argument Collections

Let's enhance our rocket launcher to support multiple planets.

```c#
public void LaunchRocket(
    IEnumerable<string> planets, 
    [Option(ShortName = "c", LongName = "crew")] string[] crew)
```

This is what help information looks like

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

and since options are not positional, they can be intermixed with arguments.

```bash
dotnet example.dll LaunchRocket mars -c aaron earth -c alex jupiter

launching rocket
planets: mars,earch,jupiter
crew: aaron,alex
```

!!!Caveat
    There can be only 1 operand list per command, but you can have many option lists.

## Piping

CommandDotNet provides middleware [to pipe input to operand collections](../Middleware/piped-arguments.md).

If your parameter type is `IEnumerable<T>`, the operands will be streamed into the command.