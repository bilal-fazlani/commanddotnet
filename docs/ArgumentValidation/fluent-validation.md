# FluentValidation

Use [FluentValidation](https://github.com/JeremySkinner/FluentValidation) with CommandDotNet to validate [argument models](../Arguments/argument-models.md).

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.FluentValidation
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.FluentValidation
    ```

Enable with `appRunner.UseFluentValidation()`, or `appRunner.UseFluentValidation(showHelpOnError: true)` <br/> to print help when there are validation errors.

## Example

<!-- snippet: fluent_validation -->
<a id='snippet-fluent_validation'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner =>
        new AppRunner<Program>()
            .UseNameCasing(Case.LowerCase)
            .UseFluentValidation();

    public void Create(IConsole console, Table table, Host host, Verbosity verbosity)
    {
        console.WriteLine($"created {table.Name} as {host.Server}. notifying: {table.Owner}");
    }
}

public class Host : IArgumentModel
{
    [Option]
    public string Server { get; set; }
}

public class Table : IArgumentModel
{
    [Operand]
    public string Name { get; set; }

    [Option]
    public string Owner { get; set; }
}

public class Verbosity : IArgumentModel
{
    [Option('q')]
    public bool Quiet { get; set; }
    [Option('v')]
    public bool Verbose { get; set; }
}

public class HostValidator : AbstractValidator<Host>
{
    public HostValidator()
    {
        RuleFor(h => h.Server)
            .NotNull().NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("sever is not a valid fully-qualified http, https, or ftp URL");
    }
}

public class TableValidator : AbstractValidator<Table>
{
    public TableValidator()
    {
        RuleFor(t => t.Name).NotNull().NotEmpty().MaximumLength(10);
        RuleFor(t => t.Owner).NotNull().NotEmpty().EmailAddress();
    }
}

public class VerbosityValidator : AbstractValidator<Verbosity>
{
    public VerbosityValidator()
    {
        When(v => v.Verbose,
            () => RuleFor(v => v.Quiet)
                .NotEqual(true)
                .WithMessage("quiet and verbose are mutually exclusive. There can be only one!"));
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Validation/Fluent_Validation.cs#L15-L84' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluent_validation' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: fluent_validation_create_invalid -->
<a id='snippet-fluent_validation_create_invalid'></a>
```bash
$ dotnet table.dll create TooLongTableName --server bossman --owner abc -qv
'Table' is invalid
  The length of 'Name' must be 10 characters or fewer. You entered 16 characters.
  'Owner' is not a valid email address.
'Host' is invalid
  sever is not a valid fully-qualified http, https, or ftp URL
'Verbosity' is invalid
  quiet and verbose are mutually exclusive. There can be only one!
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/fluent_validation_create_invalid.bash#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluent_validation_create_invalid' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If the validation fails, app exits with return code 2 and outputs validation error messages to error output.

## Improve Perf

CommandDotNet tries to get the valdiator for each model from `CommandContext.DependencyResolver`. 
If not found, the assembly of the model is scanned (only once).

Register your validators with a container or provide a factory method.

<!-- snippet: fluent_validation_factory -->
<a id='snippet-fluent_validation_factory'></a>
```c#
public static AppRunner AppRunner =>
    new AppRunner<Program>()
        .UseNameCasing(Case.LowerCase)
        .UseFluentValidation(validatorFactory: model =>
        {
            switch (model)
            {
                case Host: return new HostValidator();
                case Table: return new TableValidator();
                case Verbosity: return new VerbosityValidator();
                default: return null;
            }
        });
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Validation/Fluent_Validation.cs#L90-L104' title='Snippet source file'>snippet source</a> | <a href='#snippet-fluent_validation_factory' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## vs. DataAnnotations

Comparing this example with the same example in [DataAnnotations](data-annotations-validation.md), the DataAnnotations is simpler to implement, requires less code, is easier to understand and has a cleaner error message your users.
