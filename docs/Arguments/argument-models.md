# Argument Models

Argument models provide a way to define arguments in a class, allowing arguments to be reused and more easily passed to other methods.

Let's consider this notify command.

<!-- snippet: argument_models_notify_without_model -->
<a id='snippet-argument_models_notify_without_model'></a>
```c#
public void Notify(string message, List<string> recipients,
    [Option] bool dryrun, [Option('v')] bool verbose, [Option('q')] bool quiet)
{
    // send notification
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L16-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_without_model' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_models_notify_without_model_help -->
<a id='snippet-argument_models_notify_without_model_help'></a>
```bash
$ myapp.exe Notify --help
Usage: myapp.exe Notify [options] <message> <recipients>

Arguments:

  message                <TEXT>

  recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --verbose

  -q | --quiet
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/argument_models_notify_without_model_help.bash#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_without_model_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In some cases, `dryrun`, `verbose`, and `quite` will be infrastructure concerns. For example...

* A UnitOfWork middleware could commit or abort a transaction based on the `dryrun` value. 
* A Logging middleware could set the log level based on the `verbose` and `quiet` values. 
* We may want to assert `verbose` and `quiet` are mutually exclusive.

If we have multiple commands that use these same values, it's possible for them to be configured differently across commands. Take `dryrun` for example. Ask 5 different developers to add a dryrun option and you'll end up with 5 different casings for it. dryrun, dry-run, DryRun, Dryrun, ....

Use argument models to more easily reuse arguments, enforce consistency and make arguments easier to access from middleware.

Here's how they're configured

<!-- snippet: argument_models_notify_with_model -->
<a id='snippet-argument_models_notify_with_model'></a>
```c#
public void Notify(
    NotificationArgs notificationArgs,
    DryRunOptions dryRunOptions, 
    VerbosityOptions verbosityOptions)
{
    // send notification
}

public class NotificationArgs : IArgumentModel
{
    [Operand]
    public string Message { get; set; } = null!;

    [Operand]
    public List<string> Recipients { get; set; } = null!;
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L27-L44' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_model' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_models_dry_run_and_verbosity -->
<a id='snippet-argument_models_dry_run_and_verbosity'></a>
```c#
public class DryRunOptions : IArgumentModel
{
    [Option("dryrun")]
    public bool IsDryRun { get; set; } = false;
}

public class VerbosityOptions : IArgumentModel, IValidatableObject
{
    [Option('v')]
    public bool Verbose { get; set; }

    [Option('q')]
    public bool Quite { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // use the CommandDotNet.DataAnnotations package to run this validation
        if (Verbose && Quite)
            yield return new ValidationResult("Verbose and Quiet are mutually exclusive. Choose one or the other.");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L167-L189' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_dry_run_and_verbosity' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_models_notify_with_model_help -->
<a id='snippet-argument_models_notify_with_model_help'></a>
```bash
$ myapp.exe Notify --help
Usage: myapp.exe Notify [options] <Message> <Recipients>

Arguments:

  Message                <TEXT>

  Recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --Verbose

  -q | --Quite
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/argument_models_notify_with_model_help.bash#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_model_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the help output is the same, except for the casing of the names derived from the properties. The casing can be fixed by providing a name in the Operand and Option attributes, using lowercase property names, or using the [NameCasing](../OtherFeatures/name-casing.md) package to convert all command and argument names to the same case.

!!! Tip
    See [Nullable Reference Types](../TipsFaqs/nullable-reference-types.md) for avoiding  "Non-nullable property is uninitialized" warnings in your argument models

## Composition

An `IArgumentModel` can be composed from other `IArgumentModel`s allowing easy reuse of common arguments.

Using same example from above, we configure the arguments into a single model like this...

<!-- snippet: argument_models_notify_with_model_composed -->
<a id='snippet-argument_models_notify_with_model_composed'></a>
```c#
public void Notify(NotificationArgs notificationArgs)
{
    // send notification
}

public class NotificationArgs : IArgumentModel
{
    [Operand]
    public string Message { get; set; } = null!;

    [Operand]
    public List<string> Recipients { get; set; } = null!;

    public DryRunOptions DryRunOptions { get; set; } = null!;

    public VerbosityOptions VerbosityOptions { get; set; } = null!;
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L49-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_model_composed' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_models_notify_with_model_composed -->
<a id='snippet-argument_models_notify_with_model_composed'></a>
```c#
public void Notify(NotificationArgs notificationArgs)
{
    // send notification
}

public class NotificationArgs : IArgumentModel
{
    [Operand]
    public string Message { get; set; } = null!;

    [Operand]
    public List<string> Recipients { get; set; } = null!;

    public DryRunOptions DryRunOptions { get; set; } = null!;

    public VerbosityOptions VerbosityOptions { get; set; } = null!;
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L49-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_model_composed' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Accessing from interceptors and middleware

Instead of defining the model in each command method, the model could be defined in an interceptor of the root command and be available for all commands.

<!-- snippet: argument_models_notify_with_interceptor -->
<a id='snippet-argument_models_notify_with_interceptor'></a>
```c#
public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext ctx,
    DryRunOptions dryRunOptions, VerbosityOptions verbosityOptions)
{
    IEnumerable<IArgumentModel> models = ctx.InvocationPipeline.All
        .SelectMany(s => s.Invocation.FlattenedArgumentModels);
    return next();
}

public void Notify(NotificationArgs notificationArgs)
{
    // send notification
}

public class NotificationArgs : IArgumentModel
{
    [Operand]
    public string Message { get; set; } = null!;

    [Operand]
    public List<string> Recipients { get; set; } = null!;
}

public class DryRunOptions : IArgumentModel
{
    [Option("dryrun", AssignToExecutableSubcommands = true)]
    public bool IsDryRun { get; set; } = false;
}

public class VerbosityOptions : IArgumentModel
{
    [Option('v', AssignToExecutableSubcommands = true)]
    public bool Verbose { get; set; }

    [Option('q', AssignToExecutableSubcommands = true)]
    public bool Quite { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L72-L111' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_interceptor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_models_notify_with_interceptor_help -->
<a id='snippet-argument_models_notify_with_interceptor_help'></a>
```bash
$ myapp.exe Notify --help
Usage: myapp.exe Notify [options] <Message> <Recipients>

Arguments:

  Message                <TEXT>

  Recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --Verbose

  -q | --Quite
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/argument_models_notify_with_interceptor_help.bash#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_interceptor_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the use of `AssignToExecutableSubcommands=true` in the Option attributes. 
This configures the option as if defined in the command methods. 
Without this setting, the user would provide the options in the command hosting the interceptor method.

Notice in the interceptor method how the list of all argument models can be retrieved from the pipeline of the targetted command. Middleware can use this to fetch an argument model.

## Guaranteeing the order of arguments

Prior to version 4, argument position is not guaranteed to be consistent because the .Net Framework does not guarantee the order properties are reflected.

> The [GetProperties](https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperties) method does not return properties in a particular order, such as alphabetical or declaration order. Order can differ on each machine the app is deployed to. Your code must not depend on the order in which properties are returned because that order is no guaranteed.

For `Operands`, which are positional arguments, this can result in commands with operands in a non-deterministic order.

This is not reliability issue with `Option` because options are named, not positional. The only impact is the order options appear in help.

As of version 4, CommandDotNet can guarantee all arguments will maintain their position as defined within a class as long as the properties are decorated with `OperandAttribute`, `OptionAtribute` or `OrderByPositionInClassAttribute`.

### How to use

The `OperandAttribute` and `OptionAtribute` define an optional constructor parameter called `__callerLineNumber`. This uses the [CallerLineNumberAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerlinenumberattribute?view=netframework-4.8) to auto-assign the line number in the class. **Do Not** provide a value for this parameter.

CommandDotNet will raise an exception when the order cannot be determined, which occurs when either

1. `AppSettings.Arguments.DefaultArgumentMode == ArgumentMode.Operand` (the default) and the property is not attributed
1. When a nested argument model containing an operand is not decorated with `[OrderByPositionInClass]`

An example of invalidly nesting an IArgumentModel that contains operands

<!-- snippet: argument_models_notify_with_invalid_nested_operands_model -->
<a id='snippet-argument_models_notify_with_invalid_nested_operands_model'></a>
```c#
public class NotifyModel : IArgumentModel
{
    public NotificationArgs NotificationArgs { get; set; }
    public DryRunOptions DryRunOptions { get; set; }
    public VerbosityOptions VerbosityOptions { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L148-L155' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_invalid_nested_operands_model' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And the error received because NotificationArgs contains operands

<!-- snippet: argument_models_notify_with_invalid_nested_operands_help -->
<a id='snippet-argument_models_notify_with_invalid_nested_operands_help'></a>
```bash
$ myapp.exe Notify --help
CommandDotNet.InvalidConfigurationException: Operand property must be attributed with OperandAttribute or OrderByPositionInClassAttribute to guarantee consistent order. Properties:
  CommandDotNet.DocExamples.Arguments.Arguments.Argument_Models+Program_WithInvalidhNestedOperands+NotifyModel.NotificationArgs
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/argument_models_notify_with_invalid_nested_operands_help.bash#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_invalid_nested_operands_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This is the correct way to nest a model with operands

<!-- snippet: argument_models_notify_with_nested_operands_model -->
<a id='snippet-argument_models_notify_with_nested_operands_model'></a>
```c#
public class NotifyModel : IArgumentModel
{
    [OrderByPositionInClass]
    public NotificationArgs NotificationArgs { get; set; }
    public DryRunOptions DryRunOptions { get; set; }
    public VerbosityOptions VerbosityOptions { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Models.cs#L121-L129' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_models_notify_with_nested_operands_model' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
 
### Recommendation 
* When possible, do not define operands in nested argument models.
* Always attribute operands in properties with `[Operand]`.
