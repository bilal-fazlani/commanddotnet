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

<!-- snippet: data_annotations_validation -->
<a id='snippet-data_annotations_validation'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner =>
        new AppRunner<Program>()
            .UseNameCasing(Case.LowerCase)
            .UseDataAnnotationValidations();

    public void Create(IConsole console, Table table, [Option, Url] string server, Verbosity verbosity)
    {
        console.WriteLine($"created {table.Name} as {server}. notifying: {table.Owner}");
    }
}

public class Table : IArgumentModel
{
    [Operand, Required, MaxLength(10)]
    public string Name { get; set; }

    [Option, Required, EmailAddress]
    public string Owner { get; set; }
}

public class Verbosity : IArgumentModel, IValidatableObject
{
    [Option('q')]
    public bool Quiet { get; set; }
    [Option('v')]
    public bool Verbose { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Quiet && Verbose)
            yield return new ValidationResult("quiet and verbose are mutually exclusive. There can be only one!");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Validation/Data_Annotations_Validation.cs#L13-L51' title='Snippet source file'>snippet source</a> | <a href='#snippet-data_annotations_validation' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

DataAnnotations can be defined on the method parameters and model properties. Notice 

* the `Url` validation on the server option
* `Required`, `MaxLength`, and `EmailAddress` in the Table model
* `IValidatableObject` implementation of the Verbosity model.

If the validation fails, the application exits with return code 2 and outputs validation error messages to error output.

<!-- snippet: data_annotations_validation_create_invalid -->
<a id='snippet-data_annotations_validation_create_invalid'></a>
```bash
$ dotnet table.dll create TooLongTableName --server bossman --owner abc -qv
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
quiet and verbose are mutually exclusive. There can be only one!
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/data_annotations_validation_create_invalid.bash#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-data_annotations_validation_create_invalid' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
