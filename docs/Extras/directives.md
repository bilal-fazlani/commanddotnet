# Directives

Directives are special arguments enabling cross cutting features.  We've loosely followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to provide two directives: Debug & Parse


## TLDR, How to enable 
Enable the feature with `appRunner.UseDebugDirective()` and `appRunner.UseParseDirective()` or `appRunner.UseDefaultMiddleware()` for both.

Directives must be the first arguments, surrounded by square brackets and will be removed from the arguments list during tokenization.

To disable parsing directives, set `AppSettings.DisableDirectives = true`

## Debug

Sometimes you just need to debug into a process and configuring the debug arguments in VS is too many extra steps.

When you specify `[debug]`, the process id will output to the console and wait for you to attach a debugger.

```bash
$ dotnet example.dll [debug] Add 1 2

Attach your debugger to process 24236 ({exe name}).
```

The debugger is attached early in the middleware pipeline, after the args have been tokenized. 
Any logic before appRunner.Run will occur before stepping into the debugger.
To attach immediately in the Main method, use `Debugger.AttachIfDebugDirective(args)`.

```c#

    class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);
            
            // configuraation code here

            new AppRunner<MyApp>().Run(args);
        }
    }
```

## Parse

Rules for including spaces in arguments and escaping special characters differ from shell to shell.  Sometimes it's not clear why arguments are not mapping correctly.

When you specify `[parse]`, the process will exit immediately after printing each argument onto a new line.  This enables you to catch cases where the shell did not parse your arguments as expected.

```bash
$ dotnet example.dll [parse] LaunchBigRocket mars -c aaron earth -c alex jupiter
use [parse:verbose] to see results after each transformation
>>> from shell
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Option   : -c
  Value    : aaron
  Value    : earth
  Option   : -c
  Value    : alex
  Value    : jupiter
>>> transformed after: expand-response-files > expand-clubbed-flags > split-option-assignment
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Option   : -c
  Value    : aaron
  Value    : earth
  Option   : -c
  Value    : alex
  Value    : jupiter
```

Use `[parse:verbose]` to see changes after each `TokenTransformation`(expand-response-files, expand-clubbed-flags, split-option-assignments) to pinpoint which transformation resulted in unexpected changes.

```bash
$ dotnet example.dll [parse:verbose] LaunchBigRocket mars -c aaron earth -c alex jupiter
>>> from shell
  Directive: [parse:verbose]
  Value    : LaunchBigRocket
  Value    : mars
  Option   : -c
  Value    : aaron
  Value    : earth
  Option   : -c
  Value    : alex
  Value    : jupiter
>>> no changes after: expand-response-files
>>> no changes after: expand-clubbed-flags
>>> no changes after: split-option-assignments
```

## Custom directives

Directives are middleware components.  
See the [Debug](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Directives/DebugDirective.cs) 
and [Parse](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Directives/ParseDirective.cs) directives
for examples. 

The presence of a directive is checked with `commandContext.Tokens.TryGetDirective("debug", out _))`.
