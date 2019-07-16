## Installation

CommandDotNet can be installed from [nuget.org](https://www.nuget.org/packages/CommandDotNet/)

``` bash tab=".NET CLI"
dotnet add package CommandDotNet
```

``` bash tab="Nuget Package Manager"
Install-Package CommandDotNet
```

## Let's build a calculator

Let's say you want to create a calculator console application which can perform 2 operations:

1. Addition
2. Subtraction

It prints the results on console.

Let's begin with creating the class

```c#
public class Calculator
{
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }

    public void Subtract(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 - value2}");
    }
}
```

Now that we have our calculator ready, let's see about how we can call it from command line.


```c#
class Program
{
    static int Main(string[] args)
    {
        AppRunner<Calculator> appRunner = new AppRunner<Calculator>();
        return appRunner.Run(args);
    }
}
```

Assuming our application's name is `example.dll`

let's try and run this app from command line using dotnet

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  Add
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Voila!

So, as you might have already guessed, it is detecting methods of the calculator class. How about adding some helpful description.

```c#
[ApplicationMetadata(Description = "Adds two numbers. duh!")]
public void Add(int value1, int value2)
{
    Console.WriteLine($"Answer: {value1 + value2}");
}
```

This should do it.

Let's see how the help appears now.

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  Add        Adds two numbers. duh!
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Awesome. Descriptions are not required but can be very useful depending upon the complexity of your app and the audience. 

Now let's try to see if we can get further help for the add command.

INPUT

```bash
dotnet example.dll Add --help
```

OUTPUT

```bash
Usage: dotnet example.dll Add [arguments] [options]

Arguments:
  value1  Int32
  value2  Int32

Options:
  -h | -? | --help  Show help information
```

tada!

Ok, so here, it show what parameters are required for addition and their type.

Let's try and add two numbers.

INPUT

```bash
dotnet example.dll Add 40 20
```

OUTPUT

```bash
Answer: 60
```

Cool. You get the gist of this library. Let's move on.