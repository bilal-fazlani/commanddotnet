# Command Logger

Directives are special arguments enabling cross cutting features.  We've loosely followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to provide two directives: Debug & Parse

Directives are a great way to add troubleshooting tools to your application. See [Custom Directives](#custom-directives) at the bottom of this page for tips on adding your own.


## TLDR, How to enable 
1. Add nuget package [CommandDotNet.CommandLogger](https://www.nuget.org/packages/CommandDotNet.CommandLogger)
1. Enable the feature with `appRunner.UseCommandLogger()`

## Command logging

This feature logs information about the command just before the command method is executed.

The information logged includes

### Writer factory

By default, information will be logged to `CommandContext.Console.Out` for all commands. Use writerFactory to 

1. Change the target output: `writerFactory: ctx => Log.Info`
1. Select which commands to log: `writerFactory: ctx => ctx.ParseResult.TargetCommand.CustomAttributes.HasAttribute<MyAttribute>()`

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
```

### System information

System information can be included by setting `includeSystemInfo: true`. This is the default set.

```bash
Tool version  = testhost.dll 16.2.0
.Net version  = .NET Core 4.6.26614.01
OS version    = Microsoft Windows 10.0.18363
Machine       = MyComputer
Username      = MyComputer\Me
```

Additional information can be provided by setting the `additionalInfoCallback` parameter with a 
`Func<CommandContext, IEnumerable<(string key, string value)>>`. This allows including configuration state
from the CommandContext.

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