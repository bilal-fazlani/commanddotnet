# Validation

CommandDotNet has packages to utilize the [DataAnnotations](../ArgumentValidation/data-annotations-validation.md) and [FluentValidation](../ArgumentValidation/fluent-validation.md) frameworks.

Let's set the support for DataAnnotations and how you can use [Argument Models](../Arguments/argument-models.md) to reuse argument definitions and validations.

<!-- snippet: dataannotations-1-table -->
<a id='snippet-dataannotations-1-table'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => 
        new AppRunner<Program>()
            .UseNameCasing(Case.LowerCase)
            .UseDataAnnotationValidations();
    
    public Task<int> Interceptor(InterceptorExecutionDelegate next, Verbosity verbosity)
    {
        // pre-execution logic here

        return next(); // Create method is executed here

        // post-execution logic here
    }

    public void Create(IConsole console, Table table, [Option, Url] string server)
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
    [Option('s', AssignToExecutableSubcommands = true)]
    public bool Silent { get; set; }
    [Option('v', AssignToExecutableSubcommands = true)]
    public bool Verbose { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Silent && Verbose)
            yield return new ValidationResult("silent and verbose are mutually exclusive. There can be only one!");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/DataAnnotations/DataAnnotations_1_Table.cs#L14-L61' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations-1-table' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There's a lot more going on here. so let's break it down. 

This app has one command `create`, defined by the `Create` command. 

* The command name and all argument names are converted to lowercase thanks to `.UseNameCasing(Case.LowerCase)`.
    * see [Name Casing](../OtherFeatures/name-casing.md) for more details.
* Defins arguments via `server` parameter and `Table` argument model

This app also has an interceptor method that 

* Wraps the execution of all child commands.
    * This is one way to implement pre/post hooks for a set of commands. The other is via [Middleware](../Extensibility/middleware.md) components.
* Defines arguments via `Verbosity` that can be reused across all subcommands.
    * Silent and Verbose have `AssignToExecutableSubcommands=true` which means the options will appear as options for each subcommand. If this had been false, they would appear for the parent command.
    * This is an example of how to enforce consistency for cross-cutting concerns in your application.
    * Demonstrates use of IValidatableObject for complex object validation

ArgumentModels can contain other ArgumentModels and validation will be run at all levels.

Here is the help for this command. We haven't defined any descriptions or such. Notice validation logic is not shown in the help. This is something you could add with additional middleware. We also accept feature contributions :).

<!-- snippet: dataannotations-1-table-create-help -->
<a id='snippet-dataannotations-1-table-create-help'></a>
```bash
$ dotnet table.dll create --help
Usage: dotnet table.dll create [options] <name>

Arguments:

  name  <TEXT>

Options:

  --owner         <TEXT>

  --server        <TEXT>

  -s | --silent

  -v | --verbose
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/DataAnnotations-1-table-create-help.bash#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations-1-table-create-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When the command is run with invalid arguments, you get the resulting error messages. Notice we were able to club (aka bundle) the `Silent` and `Verbose` shortnames `-sv`. 

<!-- snippet: dataannotations-1-table-create -->
<a id='snippet-dataannotations-1-table-create'></a>
```bash
$ dotnet hr.dll create TooLongTableName --server bossman --owner abc -sv
silent and verbose are mutually exclusive. There can be only one!
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/DataAnnotations-1-table-create.bash#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-dataannotations-1-table-create' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
