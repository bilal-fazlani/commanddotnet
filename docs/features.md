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

- [x] Command Interception
    - interceptors for subcommands 
    - middleware for all commands

- [x] Method Interception
> CommandContext.InvocationPipeline to access method and params

</div>

<div markdown="1" class="feature">

## Data Types

- [x] Primitives, Enums & `Nullable<T>`
- [x] Collections
- [ ] Dictionaries
- [x] Password: mask value in logs and output with `*****`
- [x] Any type with a string constructor
- [x] Any type with a static Parse(string) method
- [x] Any type with a TypeConverter
- [x] Custom Type Descriptors
> Customize parsing and the type name shown in help and
- [x] Define allowed values by type
> Allowed values are shown in help and will soon be used for suggestions

</div>

<div markdown="1" class="feature">

## Help	
- [ ] Autocomplete
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

## Localization (beta)

- [x] `IStringLocalizer` pattern
- [x] resx translation files
> currently english only. PRs accepted
- [x] json translation pattern
> currently english only. PRs accepted
- [x] Culture directive
> set the culture a command will run in
- [x] Test support

</div>

<div markdown="1" class="feature">

## Extensibility

- [x] Custom middleware
- [x] Custom directives
- [x] Custom token transformations
- [x] Custom parameter resolvers

</div>
</div>

<div markdown="1" class="feature-column">
<div markdown="1" class="feature">

## Arguments

- [x] Positional (operands)
- [x] Named (options)
    - Short and long names
    - Flags (with bundling/clubbing feature)
- [x] Define arguments as parameters in methods
- [x] Define arguments in properties in POCOs

</div>

<div markdown="1" class="feature">

## Argument Values

- [x] Response Files
- [x] Piped Input with streaming
- [x] Prompts
    - Hide passwords
    - Multi-entry for collections
    - Auto prompt for missing arguments (optional)
- [x] Default from EnvVar
- [x] Default from AppSetting

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
- [x] TestConsole
- [x] TestDependencyResolver
> new TestDependencyResolver{ dbSvc, httpSvc }
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

<div markdown="1" class="feature">

## Other

- [x] ++ctrl+c++
- [x] Name casing
> consistent name casing via Humanizer
- [x] Spectre AnsiConsole
> ansi, colors, markup syntax, prompting, progress bars, tables. live displays, test capture, and much more

</div>

</div>
</div>