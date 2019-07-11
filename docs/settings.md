When you create a new instance of `AppRunner<T>` you can pass an instance new `AppSettings` object.
Here are the settings you can change:

### Show argument details

Shows type information of arguments in help text. Enabled by default.

### Method argument mode

Possible values : 

1. Parameter (default)
2. Option

When method argument mode is set to parameter, all arguments of methods are treated as parameters and dont need any names to be passed through command line.
Note that order of passing parameters matter in this mode.

When method argument mode is set to option, all arguments of methods are treated as options and need a name to be passed.

Note that this is only applicable for methods and not constructors. For constructors, all arguments are options only.

### Enable version option

True by default. It adds an additional option to the root command. It shows version of the application.

### Case

```c#
public class SomeClass
{
    public SomeClass(string Url)
    {
        
    }
    
    public void ProcessRequest()
    {
        
    }
}
```

by default this would result into something like this:

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information
  --Url             String

Commands:
  ProcessRequest

Use "dotnet example.dll [command] --help" for more information about a command.
```

Command line conventions are different from C# convetions and the usual pascal casing of method names or camel casing of parameter names may not be suitable for command line arguments.

You can continue to develop you classes and method in normal C# conventions and tell library to tranform them into the desired casing.

There are 5 modes available:

1. DontChange
1. LowerCase 
1. CamelCase 
1. KebabCase 
1. PascalCase

If you now use a different setting,

```c#
class Program
{
    static int Main(string[] args)
    {
        AppRunner<SomeClass> appRunner = new AppRunner<SomeClass>(new AppSettings
        {
            Case = Case.KebabCase
        });
        return appRunner.Run(args);
    }
}
```

The result would something like this:

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information
  --url             String

Commands:
  process-request

Use "dotnet example.dll [command] --help" for more information about a command.
```

Note that this would not tranfsorm any name that you have overridden via `[ApplicationMetadata]`, `[Option]` or `[Argument]` attributes.

### Boolean mode

In this library, there are two ways to parse boolean [Options](#options). Note that this is not applicable for [Parameters](#parameters).

#### 1.  **Implicit**

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

##### Flag clubbing

    Clubbing of one letter [options](#options) is supported. For example,

    If a command has multiple [boolean flags](#boolean-mode) [options](#options) like:

    ```c#
    public void Print([Option(ShortName="c")]bool qwerty, bool e, bool x){ }
    ```

    These can be passed either as

    ```bash
    dotnet example.dll print -c -e -x
    ```

    OR

    ```bash
    dotnet example.dll print -ecx
    ```


#### 2. Explicit

    If you want users to explicitly enter true or false, you need to set the boolean mode explicit. You can do that, by using the `[Argument]` attribute as shown below:

    ```c#
    public void MyCommand([Argument(BooleanMode = BooleanMode.Explicit)]bool capturelogs)
    {
    
    }
    ```

    Note that you can only set `BooleanMode = BooleanMode.Explicit` or even `BooleanMode = BooleanMode.Explicit` for `bool` / `bool?` type parameters.

    When you use explicit boolean mode, these scenarios are valid:

    ```bash
    dotnet example.dll MyCommand
    dotnet example.dll MyCommand --capturelogs false
    dotnet example.dll MyCommand --capturelogs true
    ```

    but `dotnet example.dll MyCommand --capturelogs` is not valid and will result into error. It will only work in Implicit boolean mode.


    When you check the help of a command, you if you see `Boolean` it means if you wan't to make it true, you need to pass an explit value. If you don't pass one, it will default to `false` automatically. Implicit and explicit are just ways to pass the value, under the hood they are just boolean parameters.
