# Command Logger

## TLDR, How to enable 
1. Enable the feature with `appRunner.UseCommandLogger()`

## Command logging

This feature logs information about the command just before the command method is executed.

The default behavior registers a `[cmdlog]` directive that outputs to the console using `CommandContext.Console.Out`.

## Default Behavior

Given a program with default configuration

<!-- snippet: command_logger -->
<a id='snippet-command_logger'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner =>
        new AppRunner<Program>().UseCommandLogger();

    public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Command_Logger.cs#L14-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When the `[cmdlog]` directive is specified, the command logger middleware will output 

* original input from the command line
* target command
* parsed arguments with source of value
* some basic system information

<!-- snippet: command_logger_default_directive -->
<a id='snippet-command_logger_default_directive'></a>
```bash
$ example.exe [cmdlog] Add 1 1

***************************************
Original input:
  [cmdlog] Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
***************************************
2
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_default_directive.bash#L1-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_default_directive' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Configuration

Here are the available parameters for configuration:

<!-- snippet: UseCommandLogger-parameters -->
<a id='snippet-usecommandlogger-parameters'></a>
```c#
/// <summary>Enable the command logger middleware</summary>
/// <param name="appRunner">The <see cref="AppRunner"/></param>
/// <param name="writerFactory">
/// Provide an action to capture the command logger output.
/// When the action is null,
/// the command will not be logged
/// When the parameter is null,
/// the `[cmdlog]` directive is enabled with Console.Out as the target
/// </param>
/// <param name="excludeSystemInfo">Exclude OS, .net version and tool version</param>
/// <param name="includeAppConfig">Prints the entire app configuration</param>
/// <param name="includeMachineAndUser">Include machine name, username</param>
/// <param name="additionalInfoCallback">Additional information to include.</param>
public static AppRunner UseCommandLogger(this AppRunner appRunner,
    Func<CommandContext, Action<string?>?>? writerFactory = null,
    bool excludeSystemInfo = false,
    bool includeAppConfig = false,
    bool includeMachineAndUser = false,
    Func<CommandContext, IEnumerable<(string key, string value)>?>? additionalInfoCallback = null)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/AppRunnerConfigExtensions.cs#L249-L269' title='Snippet source file'>snippet source</a> | <a href='#snippet-usecommandlogger-parameters' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Include Machine and User

<!-- snippet: command_logger_include_machine_and_user -->
<a id='snippet-command_logger_include_machine_and_user'></a>
```c#
.UseCommandLogger(
    excludeSystemInfo: true,
    includeMachineAndUser: true);
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Command_Logger.cs#L75-L79' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_include_machine_and_user' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: command_logger_include_machine_and_user_exe -->
<a id='snippet-command_logger_include_machine_and_user_exe'></a>
```bash
$ example.exe [cmdlog] Add 1 1

***************************************
Original input:
  [cmdlog] Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

Machine   = my-machine
Username  = \my-machine\username
***************************************
2
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_include_machine_and_user_exe.bash#L1-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_include_machine_and_user_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Additional information can be provided by setting the `additionalInfoCallback` parameter with a 
`Func<CommandContext, IEnumerable<(string key, string value)>>`.  Any CommandContext state can be included.

## Include AppConfig

<!-- snippet: command_logger_appconfig -->
<a id='snippet-command_logger_appconfig'></a>
```c#
.UseCommandLogger(includeAppConfig: true);
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Command_Logger.cs#L119-L121' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_appconfig' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: command_logger_appconfig_exe -->
<a id='snippet-command_logger_appconfig_exe'></a>
```bash
$ example.exe [cmdlog] Add 1 1

***************************************
Original input:
  [cmdlog] Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345

AppConfig:
          AppSettings:
      Arguments:         ArgumentAppSettings:
        BooleanMode: Implicit
        DefaultArgumentMode: Operand
        DefaultOptionSplit: 
        DefaultPipeTargetSymbol: ^
        SkipArityValidation: False
      ArgumentTypeDescriptors: ArgumentTypeDescriptors:
        ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.BoolTypeDescriptor
        ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.EnumTypeDescriptor
        ErrorReportingDescriptor > DelegatedTypeDescriptor<String>: 'Text'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Password>: 'Text'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Char>: 'Character'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Int64>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Int32>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Int16>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<UInt64>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<UInt32>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<UInt16>: 'Number'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Decimal>: 'Decimal'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Double>: 'Decimal'
        ErrorReportingDescriptor > DelegatedTypeDescriptor<Single>: 'Decimal'
        ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.ComponentModelTypeDescriptor
        ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.StringCtorTypeDescriptor
      Commands:         CommandAppSettings:
        InheritCommandsFromBaseClasses: False
      DisableDirectives: False
      Help:         AppHelpSettings:
        ExpandArgumentsInUsage: True
        PrintHelpOption: False
        TextStyle: Detailed
        UsageAppName: example.exe
        UsageAppNameStyle: Adaptive
      Localize: 
      Parser:         ParseAppSettings:
        AllowBackslashOptionPrefix: False
        AllowSingleHyphenForLongNames: False
        DefaultArgumentSeparatorStrategy: EndOfOptions
        IgnoreUnexpectedOperands: False
    DependencyResolver: 
    HelpProvider: CommandDotNet.Help.HelpTextProvider
    TokenTransformations:

    MiddlewarePipeline:
      <>c__DisplayClass4_1.<RunInMem>g__CaptureCommandContext|4
      AppRunner.OnRunCompleted
      HelpMiddleware.PrintHelp
      TokenizerPipeline.TokenizeInputMiddleware
      ClassModelingMiddleware.CreateRootCommand
      CommandParser.ParseInputMiddleware
      ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
      HelpMiddleware.CheckIfShouldShowHelp
      PipedInputMiddleware.InjectPipedInputToOperandList
      BindValuesMiddleware.BindValues
      ResolveCommandClassesMiddleware.ResolveCommandClassInstances
      ValidateArityMiddleware.ValidateArity
      CommandLoggerMiddleware.CommandLogger
      ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
    ParameterResolvers:
      CommandDotNet.CommandContext
      CommandDotNet.IConsole
      CommandDotNet.IEnvironment
      System.Threading.CancellationToken
***************************************
2
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_appconfig_exe.bash#L1-L92' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_appconfig_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Recipes for writerFactory

Use `writerFactory` parameter to conditionally provide a target for the log. 

If the factory returns null, the command will not be logged.

How to use:

#### Change the target output

`writerFactory: ctx => Log.Info`

Logs every command to logging framework. See [Enable via directive](#enable-via-directive) for an example of how to selectively log commands. 

!!! note
    This disables `[cmdlog]` directive enabled. See [Enable via directive](#enable-via-directive) for an example of how to enable it again.

#### Enable via attribute

<!-- snippet: command_logger_custom_attribute -->
<a id='snippet-command_logger_custom_attribute'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => new AppRunner<Program>()
        .UseCommandLogger(ctx =>
            ctx.ParseResult!.TargetCommand.HasAttribute<LogCommandAttribute>()
                ? ctx.Console.Out.Write
                : null);

    [LogCommand]
    public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);

    public void Subtract(IConsole console, int x, int y) => console.WriteLine(x - y);
}

public class LogCommandAttribute : Attribute { }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Command_Logger.cs#L222-L240' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_custom_attribute' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice Add will always log for the command and Subtract never will. 

This can be useful when you have some commands you always want logged.

<!-- snippet: command_logger_custom_attribute_enabled -->
<a id='snippet-command_logger_custom_attribute_enabled'></a>
```bash
$ example.exe Add 1 1

***************************************
Original input:
  Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
***************************************
2
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_custom_attribute_enabled.bash#L1-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_custom_attribute_enabled' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: command_logger_custom_attribute_disabled -->
<a id='snippet-command_logger_custom_attribute_disabled'></a>
```bash
$ example.exe Subtract 1 1
0
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_custom_attribute_disabled.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_custom_attribute_disabled' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

#### Enable via directive 

Allow user to enable as a [directive](../Extensibility/directives.md)

<!-- snippet: command_logger_directive -->
<a id='snippet-command_logger_directive'></a>
```c#
return ctx.Tokens.TryGetDirective("cmdlog", out _) 
    ? s => ctx.Console.Out.Write(s)
    : null;
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Diagnostics/CommandLoggerMiddleware.cs#L31-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_directive' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!!Note
    this is the default behavior.

#### Enable via root option

Add an intercepor method to your root command with a `--logcmd` option. This also makes the option visible to users via help.

<!-- snippet: command_logger_root_option -->
<a id='snippet-command_logger_root_option'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => new AppRunner<Program>();

    public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext ctx,
        [Option(
            Description = "Output the command with arguments and system info", 
            BooleanMode = BooleanMode.Implicit)] bool logcmd)
    {
        if (logcmd)
        {
            CommandLogger.Log(ctx);
        }
        return next();
    }
    
    public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);

    public void Subtract(IConsole console, int x, int y) => console.WriteLine(x - y);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Command_Logger.cs#L278-L301' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_root_option' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: command_logger_root_option_exe -->
<a id='snippet-command_logger_root_option_exe'></a>
```bash
$ example.exe --logcmd Add 1 1

***************************************
Original input:
  --logcmd Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

options:

  logcmd <flag>
    value: True
    inputs: true (from: --logcmd)
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
***************************************
2
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/command_logger_root_option_exe.bash#L1-L34' title='Snippet source file'>snippet source</a> | <a href='#snippet-command_logger_root_option_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
