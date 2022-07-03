// begin-snippet: command_logger_appconfig_exe
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
      Localization:         LocalizationAppSettings:
        Localize: 
        UseMemberNamesAsKeys: False
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
// end-snippet