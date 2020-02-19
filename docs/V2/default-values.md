C# supports default values for parameters and so does this library.

Let's make some changes to Calculator to add a new Command - `Divide`. And let's default value2 to 1 when user doesn't provide a value.
This will prevent the calculator from crashing because of `DivideByZeroException`

```c#
public void Divide(int value1, int value2 = 1)
{
    Console.WriteLine($"Answer: {value1 / value2}");
}
```

Here's how help looks like:

INPUT 
```bash
dotnet example.dll Divide --help 
```

OUTPUT

```bash
Usage: dotnet CommandDotNet.Example.dll Divide [arguments]

Arguments:

  value1    <NUMBER>

  value2    <NUMBER>    [1]
```
