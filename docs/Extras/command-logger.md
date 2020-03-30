# Command Logger

Directives are special arguments enabling cross cutting features.  We've loosely followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to provide two directives: Debug & Parse

Directives are a great way to add troubleshooting tools to your application. See [Custom Directives](#custom-directives) at the bottom of this page for tips on adding your own.


## TLDR, How to enable 
1. Enable the feature with `appRunner.UseCommandLogger()` or use `appRunner.UseDefaultMiddleware()`

## Command logging

This feature logs information about the command just before the command method is executed.

The default behavior registers a `[cmdlog]` directive that outputs to the console using `CommandContext.Console.Out`.

example usage: 

```bash
`dotnet example.com [cmdlog] add 1 2`
```

## Outputs

### Command and arguments
The command to be executed and the argument values as described in the [parse directive](directives.md#parse).

```bash
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
```

### System information

System information is included by default and can be excluded by setting `excludeSystemInfo: true`. 

The default set of information is:

```bash
Tool version  = testhost.dll 16.2.0
.Net version  = .NET Core 4.6.26614.01
OS version    = Microsoft Windows 10.0.18363
Machine       = MyComputer
Username      = MyComputer\Me
```

Additional information can be provided by setting the `additionalInfoCallback` parameter with a 
`Func<CommandContext, IEnumerable<(string key, string value)>>`.  Any CommandContext state can be included.

### AppConfig

AppConfig can be included by setting `includeAppConfig: true`. This

```bash
AppConfig:
  AppSettings:
    ArgumentTypeDescriptors: ArgumentTypeDescriptors:
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.BoolTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.EnumTypeDescriptor
      ErrorReportingDescriptor > DelegatedTypeDescriptor<String>: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Password>: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Char>: 'Character'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int64>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int32>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int16>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Decimal>: 'Decimal'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Double>: 'Double'
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.ComponentModelTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.StringCtorTypeDescriptor
    BooleanMode: Implicit
    DefaultArgumentMode: Operand
    DisableDirectives: False
    GuaranteeOperandOrderInArgumentModels: False
    Help: CommandDotNet.Help.AppHelpSettings
    IgnoreUnexpectedOperands: False
  DependencyResolver:
  HelpProvider: CommandDotNet.Help.HelpTextProvider
  TokenTransformations:
    expand-clubbed-flags
    split-option-assignments
  MiddlewarePipeline:
    TokenizerPipeline.TokenizeInputMiddleware
    ClassModelingMiddleware.CreateRootCommand
    CommandParser.ParseInputMiddleware
    ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
    HelpMiddleware.DisplayHelp
    BindValuesMiddleware.BindValues
    ResolveCommandClassesMiddleware.ResolveCommandClassInstances
    AppRunnerTestExtensions.InjectTestOutputs
    CommandLoggerMiddleware.CommandLogger
    ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
  ParameterResolvers:
    CommandDotNet.CommandContext
    CommandDotNet.Rendering.IConsole
    System.Threading.CancellationToken
```

## Replacing the default behavior

Use `writerFactory` parameter to conditionally provide a target for the log. 

If the factory returns null, the command will not be logged.

How to use:

1. Change the target output: `writerFactory: ctx => Log.Info`
    * Logs every command to logging framework. No `[cmdlog]` directive
1. Select which commands to log: `writerFactory: ctx => ctx.ParseResult.TargetCommand.HasAttribute<EnableCommandLogger>() ? ctx.Console.Out.WriteLine : (Action<string>)null`
    * Logs only commands attributed with your custom `EnableCommandLoggerAttribute`. No `[cmdlog]` directive
1. Allow user to enable as a [directive](directives.md): `writerFactory: ctx => ctx.Tokens.TryGetDirective("cmdlog", out _) ? ctx.Console.Out.WriteLine : (Action<string>)null`
    * Usage: `dotnet example.com [cmdlog] add 1 2`
    * Note: this is the default behavior.
1. Blend the options (see code below)
    * Always output to logs 
    * output to the console when... 
        * The user runs with the `[cmdlog]` directive
        * The command is attributed with your custom `EnableCommandLoggerAttribute`

```c#
appRunner.UseCommandLogger(writerFactory: ctx => 
{
    // EnableCommandLogger is just an example name you could implement
    if (ctx.Tokens.TryGetDirective("cmdlog", out string value)
        || ctx.ParseResult.TargetCommand.HasAttribute<EnableCommandLogger>())
    {
        return Log.IsInfoEnabled()
            ? log =>
            {
                Log.Info(log);
                ctx.Console.Out.WriteLine(log);
            }
            : (Action<string>)ctx.Console.Out.WriteLine;
    }

    return Log.IsInfoEnabled()
        ? Log.Info
        : (Action<string>)null;
});
```