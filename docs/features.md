---
hide:
  - navigation
  - toc
---

<style>
  .md-typeset h1 {
    margin-bottom:0.5em
  }
</style>

# Features

<div markdown="1" id="features-page">
<div markdown="1" class="feature-column">
<div markdown="1" class="feature">

## Commands

- [x] Define commands as methods and/or classes ([docs](Commands/commands.md))
    - supports the pattern you prefer: 
        - class per command
        - logically grouped commands as methods
        > similar to ASP.NET endpoints

- [x] Parameter Resolvers ([docs](Extensibility/parameter-resolvers.md))
> Inject console and other services as parameter to the command methods: IConsole, CommandContext, CancellationToken & IPrompter by default.

- [x] Infinite nesting of subcommands ([docs](Commands/subcommands.md))
    - Nested classes
    - Composed classes
    > reuse subcommands in multiple parents
    - Commands define arguments for their subcommands

- [x] Command Interception
    - via interceptor methods (aka hooks) ([docs](Extensibility/interceptors.md))
    > allows logic before and after a command or subcommands run
    - via middleware ([docs](Extensibility/middleware.md))

- [x] Method Interception
> CommandContext.InvocationPipeline to access method and params

</div>

<div markdown="1" class="feature">

## Data Types 
[docs](Arguments/argument-types.md)

- [x] Primitives, Enums, Nullable&lt;T&gt; & [Nullable Reference Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/nullable-reference-types)

- [x] Lists, Arrays and IEnumberable&lt;T&gt; for streaming

- [x] Password: mask value in logs and output with `*****` ([docs](Arguments/passwords.md))

- [x] Any type with a
    - [x] string constructor
    - [x] static Parse(string) method
    - [x] TypeConverter

- [x] Custom Type Descriptors
> Customize parsing and the type name shown in help and

- [x] Define allowed values by type
> Allowed values are shown in help and typo suggestions

</div>

<div markdown="1" class="feature">

## Help	
- [x] Typo suggestions ([docs](Help/typo-suggestions.md))

- [x] Auto generated help ([docs](Help/help.md))
> Aliases: -? -h --help

- [x] Custom help generators ([docs](Help/help.md#custom-help-provider))

</div>

<div markdown="1" class="feature">

## Dependency Injection 
[docs](OtherFeatures/dependency-injection.md)

- [x] MicrosoftDependencyInjection
- [x] Autofac
- [x] SimpleInjector
- [x] Test resolver ([docs](TestTools/overview.md#testdependencyresolver))
- [x] Custom

</div>

<div markdown="1" class="feature">

## Localization

[docs](Localization/overview.md)

- [x] supports `IStringLocalizer`

- [x] resx translation files available
> currently english only. PRs accepted

- [x] json translation files available
> currently english only. PRs accepted

- [x] `[culture]` directive ([docs](Localization/culture-directive.md))
> set the culture for a command for manual verification

- [x] Test support ([docs](Localization/testing.md))

</div>

<div markdown="1" class="feature">

## Extensibility

- [x] Custom middleware ([docs](Extensibility/middleware.md))
- [x] Custom directives ([docs](Extensibility/directives.md))
- [x] Custom token transformations ([docs](Extensibility/token-transformations.md))
- [x] Custom parameter resolvers ([docs](Extensibility/parameter-resolvers.md))

</div>


<div markdown="1" class="feature">

## Other

- [x] ++ctrl+c++ ([docs](OtherFeatures/cancellation.md))

- [x] Name casing ([docs](OtherFeatures/name-casing.md))
> consistent name casing via Humanizer

- [x] Spectre AnsiConsole ([docs](OtherFeatures/spectre.md))
> ansi, colors, markup syntax, prompting, progress bars, tables. live displays, test capture, and much more

</div>
</div>

<div markdown="1" class="feature-column">
<div markdown="1" class="feature">

## Arguments

[docs](Arguments/arguments.md)

- [x] Positional (operands)

- [x] Named (options)
    - Short and long names
    - Flags
    - bundling aka clubbing

- [x] Define arguments as parameters in methods

- [x] Define arguments as properties in POCOs
    - POCOs can be nested for easier reuse of infrastructural arguments, i.e. dryrun, verbosity, etc.

- [x] Option prefixes
    - Posix (default): `-` for short names and `--` for long names
    - Windows (optional): `/` for both short and long names
    - Powershell (optional): `-` for long names

- [x] Argument separator `--` 
    - as "end of options" indicator ([docs](ArgumentValues/argument-separator.md#end-of-options-indicator))
    - or for pass-thru arguments ([docs](ArgumentValues/argument-separator.md#pass-thru-arguments))
    - configure globally and per command

- [x] Capture unexpected arguments or throw exception ([docs](ArgumentValues/argument-separator.md#unexpected-operands))
    - configure globally and per command
</div>

<div markdown="1" class="feature">

## Argument Values

- [x] Response Files ([docs](ArgumentValues/response-files.md))

- [x] Piped Input with streaming ([docs](ArgumentValues/piped-arguments.md))
    - user can pipe to specific arguments

- [x] Negative numbers and other values starting with `-`
> Most frameworks treat these as options. We've got you covered.

- [x] split multi-value options by char ([docs](Arguments/argument-collections.md#option-collections))
    - configure char globally and per option

- [x] Prompts ([docs](ArgumentValues/prompting.md))
    - Hide passwords
    - Multi-entry for collections
    - Auto prompt for missing arguments (optional)
    - Spectre AnsiConsole integration for alternate prompting experience

- [x] Default from EnvVar ([docs](ArgumentValues/default-values-from-config.md##environment-variables))
> values show as defaults in command help

- [x] Default from AppSetting ([docs](ArgumentValues/default-values-from-config.md#appsettings))
    - using [AppSetting] attribute
    - using naming conventions
    > use command and arguments names as keys. eg. `"--verbose": "true"`
    - using IArgumentModel as configuration object

</div>

<div markdown="1" class="feature">

## Validation	
   
- [x] Arity based on property and parameter definitions ([docs](Arguments/argument-arity.md#validation))
- [x] DataAnnotations ([docs](ArgumentValidation/data-annotations-validation.md))
- [x] FluentValidation ([docs](ArgumentValidation/fluent-validation.md))

</div>

<div markdown="1" class="feature">

## Testing

- [x] BDD Framework ([docs](TestTools/Harness/bdd.md))
> Test an app as if run from the console

- [x] Supports parallel test
> the whole framework avoids static state to support parallel test runs

- [x] Test prompts ([docs](TestTools/Tools/testing-prompts.md))

- [x] IConsole and SystemConsole covering most members of System.Console ([docs](OtherFeatures/iconsole.md))
    - TestConsole to capture output and mock piped and user input
    - Intercept Console.Out and Console.Error
    - Spectre AnsiConsole support also with AnsiTestConsole
- [x] IEnvironment and SystemEnvironment covering most members of System.Environment
    - TestEnvironment and .UseTestEnv extension for tests

- [x] TestDependencyResolver ([docs](TestTools/overview.md#testdependencyresolver))
> `new TestDependencyResolver{ dbSvc, httpSvc }`

- [x] TempFiles helper ([docs](TestTools/overview.md#tempfiles))
> create and cleanup files used for tests

- [x] Capture State  ([docs](TestTools/overview.md#capturestate))
> Capture state within a run to help test custom middleware components

</div>

<div markdown="1" class="feature">

## Diagnostics	

- [x] App Version ([docs](Diagnostics/app-version.md))
> `-v` or `--version`

- [x] `[debug]` directive ([docs](Diagnostics/debug-directive.md))
> step into debugger

- [x] `[time]` directive ([docs](Diagnostics/time-directive.md))
> outputs the execution time of a command

- [x] `[parse]` directive ([docs](Diagnostics/parse-directive.md))
    - show final values
    - show inputs and source
      > original source of value, including > response file paths
    - show defaults and source
      > including key if from EnvVar or AppSetting

- [x] Command logging ([docs](Diagnostics/command-logger.md))
> show parse output and optionally system info and app config

- [x] Custom error handling ([docs](Diagnostics/exceptions.md))

</div>

</div>
</div>
