# Directives

Directives are special arguments enabling cross cutting features.  We've loosely followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to provide two directives: Debug & Parse

Directives are a great way to add troubleshooting tools to your application. See [Custom Directives](#custom-directives) at the bottom of this page for tips on adding your own.


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

Rules for including spaces in arguments and escaping special characters differ from shell to shell. Sometimes it's not clear why arguments are not mapping correctly. This problem can be compounded when using [token transformations](token-transformations.md) like [repsonse files](response-files.md) where the arguments are not easily discoverable by simply logging the arguments passed to Program.Main(string[] args)

The `[parse]` directive will display the targetted command and each argument with values and the inputs and defaults used to derive the values.

```bash
~
$ dotnet example.dll [parse] LaunchRocket mars earth jupiter @flight-plan.rsp

command: LaunchRocket

arguments:

  planets <Text>
      value: mars, earth, jupiter, mercury, venus, saturn
      inputs:
        [argument] mars, earth, jupiter
        [piped stream]
      default: source=AppSetting key=--planets: mars, jupiter

options:
  
  crew <Text>
      value: Aaron, Alex
      inputs: [prompt] Aaron, Alex
      default: Bilal

  username <Text>
      value: Bilal
      inputs:
      default: source=EnvVar key=Username: Bilal
  
  password <Text>
      value: *****
      inputs: [prompt] *****
      default: source=EnvVar key=Password: *****
  
  turbo <Flag>
      value: true
      inputs: true (from: @flight-plan.rsp -> -ts -> -t)
      default: false
  
  slingshot <Flag>
      value: true
      inputs: true (from: @flight-plan.rsp -> -ts -> -s)
      default: false
```

There are several things to notice here:

* Multiple input sources can be displayed. (planets)
* Defaults from [configs and env vars](default-values-from-config.md) are supported. (planets, username, password)
* Passwords are obscured. (password)
* Token transformations are unwrapped, making it possible to indentify exactly how an argument was provided, including any response files used. (turbo, slingshot)

To provide this mapping, the middleware pipeline must progress to the BindValues stage. 

If that stage is not reached, the parse directive will fall back to printing just the token transformations.

```bash
~
$ dotnet example.dll [parse] LaunchRocket mars earth jupiter @flight-plan.rsp

Unable to map tokens to arguments. Falling back to token transformations.

>>> from shell
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
>>> after: expand-response-files
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -ts
>>> after: expand-clubbed-flags
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -t
  Option   : -s
>>> after: split-option-assignment
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -t
  Option   : -s
```

These transformations can be appended to the regular parse output with `[parse:t]`

## Custom directives

Directives are middleware components.  
See the implementations of [Debug](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Directives/DebugDirective.cs) 
and [Parse](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Directives/ParseDirective.cs) directives
for examples.

The presence of a directive is checked with `commandContext.Tokens.TryGetDirective("debug", out _))`.  The out parameter will include the entire string within the square brackets. Using a possible logging directive `[log:debug]`, the out parameter value would be `log:debug`. You can parse the value however you'd like.
