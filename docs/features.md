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

- [x] Define commands as methods
- [x] Parameter Resolvers
> Inject console and other contexts as a parameter: IConsole, CommandContext, CancellationToken & IPrompter by default.

- [x] Infinite nesting of subcommands
    - Nested classes
    - Composed classes
    - Commands can define arguments that appear in all subcommands

- [x] Command Interception
    - via interceptor methods (aka hooks)
    > allows logic before and after a command or subcommands run
    - via middleware

- [x] Method Interception
> CommandContext.InvocationPipeline to access method and params

</div>

<div markdown="1" class="feature">

## Data Types

- [x] Primitives, Enums, Nullable&lt;T&gt; & [Nullable Reference Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/nullable-reference-types)
- [x] Lists, Arrays and IEnumberable&lt;T&gt; for streaming
- [x] Password: mask value in logs and output with `*****`
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
- [x] Typo suggestions
- [x] Auto generated help
> Aliases: -? -h --help
- [x] Custom help generators

</div>

<div markdown="1" class="feature">

## Dependency Injection

- [x] MicrosoftDependencyInjection
- [x] Autofac
- [x] SimpleInjector
- [x] Test injector
- [x] Custom

</div>

<div markdown="1" class="feature">

## Localization

- [x] supports `IStringLocalizer`
- [x] resx translation files available
> currently english only. PRs accepted
- [x] json translation files available
> currently english only. PRs accepted
- [x] `[culture]` directive 
> set the culture for a command for manual verification
- [x] Test support

</div>

<div markdown="1" class="feature">

## Extensibility

- [x] Custom middleware
- [x] Custom directives
- [x] Custom token transformations
- [x] Custom parameter resolvers

</div>


<div markdown="1" class="feature">

## Other

- [x] ++ctrl+c++
- [x] Name casing
> consistent name casing via Humanizer
- [x] Spectre AnsiConsole
> ansi, colors, markup syntax, prompting, progress bars, tables. live displays, test capture, and much more

</div>
</div>

<div markdown="1" class="feature-column">
<div markdown="1" class="feature">

## Arguments

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
</div>

<div markdown="1" class="feature">

## Argument Values

- [x] Response Files
- [x] Piped Input with streaming
- [x] Negative numbers and other values starting with `-`
> Most frameworks treat these as options. We've got you covered.
- [x] Prompts
    - Hide passwords
    - Multi-entry for collections
    - Auto prompt for missing arguments (optional)
    - Spectre AnsiConsole integration for alternate prompting experience
- [x] Default from EnvVar
> values show as defaults in command help
- [x] Default from AppSetting
    - using [AppSetting] attribute
    - using naming conventions
    - using IArgumentModel as configuration object

</div>

<div markdown="1" class="feature">

## Validation	
   
- [x] FluentValidation for argument models 
- [x] DataAnnotations

</div>

<div markdown="1" class="feature">

## Testing

- [x] BDD Framework
> Test an app as if run from the console
- [x] Supports parallel test
> the whole framework avoids static state to support parallel test runs
- [x] IConsole and SystemConsole covering most members of System.Console
    - TestConsole to capture output and mock piped and user input
    - Spectre AnsiConsole support also with AnsiTestConsole
- [x] TestDependencyResolver
> `new TestDependencyResolver{ dbSvc, httpSvc }`
- [x] TempFiles helper
> create and cleanup files used for tests
- [x] Capture State
> Capture state within a run to help test custom middleware components

</div>

<div markdown="1" class="feature">

## Diagnostics	

- [x] App Version
> `-v` or `--version`
- [x] `[debug]` directive
> step into debugger
- [x] `[time]` directive
> outputs the execution time of a command
- [x] `[parse]` directive
    - show final values
    - show inputs and source
      > original source of value, including > response file paths
    - show defaults and source
      > including key if from EnvVar or AppSetting
- [x] Command logging
> show parse output and optionally system info and app config

</div>

</div>
</div>