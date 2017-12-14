![Logo](logo.png)

# CommandDotNet

[![GitHub last commit](https://img.shields.io/github/last-commit/bilal-fazlani/CommandDotNet.svg)]()  [![Build status](https://ci.appveyor.com/api/projects/status/0q3laab22dy66sm7/branch/master?svg=true)](https://ci.appveyor.com/project/bilal-fazlani/commanddotnet/branch/master)  [![AppVeyor tests](https://img.shields.io/appveyor/tests/bilal-fazlani/CommandDotNet.svg)](https://ci.appveyor.com/project/bilal-fazlani/commanddotnet/build/tests)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/CommandDotNet.svg)](https://www.nuget.org/packages/CommandDotNet)  [![NuGet](https://img.shields.io/nuget/dt/CommandDotNet.svg)](https://www.nuget.org/packages/CommandDotNet)

Table of contents:

- [Installation](#installation)
- [Introduction](#introduction)
- [Constructor parameters](#constructor-parameters)
- [Default values](#default-values)
- [Application and Command metadata](#application-and-command-metadata)
- [Command arguments](#command-arguments)
- [Collections](#collections)
- [Supported parameter types](#supported-parameter-types)
- [Custom return codes](#custom-return-codes)
- [Default method](#default-method)
- [Boolean flags](#boolean-flags)
- Settings
- Version option
- Auto case correction
- Validations

## Installation

From nuget: https://www.nuget.org/packages/CommandDotNet


## Introduction

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
Usage: dotnet example.dll Add [options]

Options:
  -h | -? | --help  Show help information
  --value1          Int32 | Required
  --value2          Int32 | Required
```

tada!

Ok, so here, it show what parameters are required for addition and their type.

Let's try and add two numbers.

INPUT

```bash
dotnet example.dll Add --value1 40 --value2 20
```

OUTPUT

```bash
Answer: 60
```

Cool. You get the gist of this library. Let's move on.

## Constructor parameters

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
dotnet example.dll --printValues Subtract --value1 30 --value2 5
```

OUTPUT

```bash
value1 : 30, value2: 5
Answer: 25
```

**Note that you can skip to pass any parameter. It will then fallback to the default value of parameter type**

In this case, for `--printValues` it will fallback to `false` & if you don't pass either `--value1` or `--value2`, it will fallback to `0`.

**NOTE: Only one constructor is supported. If there are multiple, it will pick up first defined constructor**

## Default values

C# supports default values for parameters and so does this library.

Let's make some changes to Calculator to add a new Command - `Divide`. And let's default value2 to 1 when user doesn't provide a value.
This will prevent the calculator from crahsing because of `DivideByZeroException`

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
Usage: dotnet example.dll Divide [options]

Options:
  -h | -? | --help  Show help information
  --value1          Int32 | Required
  --value2          Int32 | Default value: 1
```

## Application and Command metadata

You can use the `[ApplicationMetadata]` attribute on the class level like this to provide details when application is called with `help` switch.

Example: 

```c#
[ApplicationMetadata(Description = "This is a crappy calculator", ExtendedHelpText = "Some more help text that appears at the bottom")]
public class Calculator
{
}
```

This attribute can also be used on a Method as shown below.

```c#
[ApplicationMetadata(Description = "Subtracts value2 from value1 and prints output", 
    ExtendedHelpText = "Again, some more detailed help text which has no meaning I still have to write to demostrate this feature",
    Name = "subtractValues")]
public void Subtract(int value1, int value2)
{
}
```

Note that when you use ApplicationMetadata attribute on a method, you can change the name of the command that is different from method name.

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
This is a crappy calculator

Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information
  --printValues     Flag

Commands:
  Add             Adds two numbers. duh!
  Divide
  subtractValues  Subtracts value2 from value1 and prints output

Use "dotnet example.dll [command] --help" for more information about a command.
Some more help text that appears at the bottom
```

## Command arguments

By default, the parameter names declared in method are the argument names. However you can change that.
By convention, an argument can have a short name and/or a longname.

Let's see an example-

```c#
public void LaunchRocket([Argument(
    LongName = "planet", 
    ShortName = "p", 
    Description = "Name of the planet you wish the rocket to go. Sorry for bad example :(")] string planetName)
{
    return;
}
```

This is what help looks like-

```bash
Usage: dotnet example.dll LaunchRocket [options]

Options:
  -h | -? | --help  Show help information
  --planet | -p     String                         Name of the planet you wish the rocket to go. Sorry for bad example :(
```

So planet name can now be passed either with `--planet` or `-p`. Specifying both is not required. 
You can specify either long name or short name or both. When none is specified, it will use parameter name by default. Description is also optional.

## Collections

Let's enhance our rocket launcher to support multiple planets.

```c#
public void LaunchRocket([Argument(ShortName = "p")] List<string> planets)
{
    return;
}
```

This is what help information looks like-

INPUT

```bash
dotnet example.dll LaunchRocket --help
```

OUTPUT

```bash
Usage: dotnet example.dll LaunchRocket [options]

Options:
  -h | -? | --help  Show help information
  -p                String (Multiple)
```

And this is how you pass multiple parameters:

```bash
dotnet example.dll LaunchRocket -p mars -p earth -p jupiter
```

## Supported parameter types

As of now, these are supported parameter types:

- `int`
- `int?`
- `long`
- `long?`
- `string`
- `bool`
- `bool?`
- `char`
- `char?`
- `List<string>`
- `List<int>`
- `List<long>`
- `List<bool>`
- `List<char>`

These are applicable for both - methods and constructor

## Custom return codes

Typically when a console app exits with no erros, it returns `0` exit code. If there there was an error, it return `1`. 
But there are many possiblities and developers use this exit code to convey details about what exactly happenned. For example,
https://msdn.microsoft.com/en-us/library/ms681381.aspx 

When you write a command line application you can return a custom return code.

I added a new method in my Calculator to accept a number as input and exit the application with that number as exit code.

```c#
[ApplicationMetadata(Description = "Return with code 5", Name = "return")]
public int ReturnCode()
{
    return 5;
}
```

So now when I call this method from console `dotnet example.dll return`, the command ends with an exit code of 5.

**Note that your main method's return type should be int for this to work**

## Default method

Right now, when you just execute the dll, without any commands, it shows help. If you want to call a method when application is executed without any 
commands, you can do that with the help of `[DefaultMethod]` attribute.

```c#
        [DefaultMethod]
        public void SomeMethod()
        {
            
        }
```

Some points to note about default method:

- It won't show up in help and can't be called explicitely with method name. The only way to execute it is not passing any command name.
- It does not support any parameters. 
- It will have access to class level fields which are passed via constructor
- It can have a return type of int or void

## Boolean flags

When you use this library, there are two ways to parse boolean parameters.

1.  **Implicit**

    This is the default mode.
    In this mode, you don't pass the value `true` or `false` in the command line. These parameters are treated as flags. They are considered `true` if they are present and `false` when they are not.

    For exampple: 
    ```bash
    dotnet example.dll --printValues
    ```

    In this case, value of parameter `printValues` will be true

    and in the following exampplem,

    ```bash
    dotnet example.dll
    ```

    value of parameter `printValues` will be false.

    Note that, when using implicit boolean mode, it will result in an error, if the user tries to explicitly enter a value for parameter. In this instance, `dotnet example.dll --printValues true` will result into an error.

    When you check the help of a command, you if you see `Flag` for a parameter, it means value is implit and does not requre an explict one.

2.  **Explicit**

    If you want users to explicitly enter true or false, you need to set the boolean mode explicit. You can do that, by using the `[Argument]` attribute as shown below:

    ```c#
    public void MyCommand([Argument(BooleanMode = BooleanMode.Explicit)]bool capturelogs)
    {
    
    }
    ```

    Note that you can only set `BooleanMode = BooleanMode.Explicit` or even `BooleanMode = BooleanMode.Explicit` for bool / bool? type parameters.

    When you use explicit boolean mode, these scenarios are valid:

    ```bash
    dotnet example.dll
    dotnet example.dll --printValues false
    dotnet example.dll --printValues true
    ```

    but `dotnet example.dll --printValues` is not valid and will result into error. It will only work in Implicit boolean mode.

When you check the help of a command, you if you see `Boolean` or `Boolean | Required` it means if you wan't to make it true, you need to pass an explit value. If you don't pass one, it will default to `false` automatically. Implicit and explicit are just ways to pass the value, under the hood they are just boolean parameters.
 
