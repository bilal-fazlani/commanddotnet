Let's say we want to add a class level field which is useful in both Addtion and Subtraction. So now the class looks something like this-

```c#
public class Calculator
{
    private readonly bool _printValues;

    public Calculator(bool printValues)
    {
        _printValues = printValues;
    }
    
    [ApplicationMetadata(Description = "Adds two numbers. duh!")]
    public void Add(int value1, int value2)
    {
        if (_printValues)
        {
            Console.WriteLine($"value1 : {value1}, value2: {value2}");
        }
        Console.WriteLine($"Answer:  {value1 + value2}");
    }

    public void Subtract(int value1, int value2)
    {
        if (_printValues)
        {
            Console.WriteLine($"value1 : {value1}, value2: {value2}");
        }
        Console.WriteLine($"Answer: {value1 - value2}");
    }
}
```

Let's see what the help command output looks like now

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information
  --printValues     Flag

Commands:
  Add        Adds two numbers. duh!
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.
```

Let's try and invoke it

INPUT 

```bash
dotnet example.dll --printValues Subtract 30 5
```

OUTPUT

```bash
value1 : 30, value2: 5
Answer: 25
```
Notes:

 - **You can skip to pass any parameter. It will then fallback to the default value of parameter type**

 - **Any parameters in constructor are [Options](#option) by default and you can't have [Argument](#argument) attribute in constructor parameters**

 - **Only one constructor is supported. If there are multiple, it will pick up first defined constructor**

### Inherited
`[Options]` attribute has a property called `Inherited`. This is particularly useful when used with constructor options. When set to true, that option is can be passed to commands as well.