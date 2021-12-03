# DataAnnotations

Use [System.ComponentModel.DataAnnotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) with CommandDotNet to validate arguments.

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.DataAnnotations
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.DataAnnotations
    ```

Enable with `appRunner.UseDataAnnotationValidations()`, or `appRunner.UseDataAnnotationValidations(showHelpOnError: true)` <br/> to print help when there are validation errors.

## Example
```c#
class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<ValidationApp>()
            .UseDataAnnotationValidations()
            .Run(args);
    }
}

public class ValidationApp
{
    public void ValidateModel(
        PersonModel person, 
        [Option, MaxLength(5)] string role)
    {
        string content = JsonConvert.SerializeObject(person, Formatting.Indented);
        Console.WriteLine(content);
    }
}

public class PersonModel : IArgumentModel
{
    [Operand, Required, Range(1, int.MaxValue)]
    public int Id { get; set; }
    
    [Operand, Required]
    public string Name { get; set; }
    
    [Option, Required, Email]
    public string Email { get; set; }
}
```

If the validation fails, app exits with return code 2 and outputs validation error messages to error output.
