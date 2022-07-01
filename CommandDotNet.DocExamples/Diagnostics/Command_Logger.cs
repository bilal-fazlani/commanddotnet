using System;
using System.Threading.Tasks;
using NUnit.Framework;
using CommandDotNet.Diagnostics;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;

namespace CommandDotNet.DocExamples.Diagnostics
{
    public class Command_Logger
    {
        public class Program_Default
        {
            // begin-snippet: command_logger
            public class Program
            {
                static int Main(string[] args) => AppRunner.Run(args);

                public static AppRunner AppRunner =>
                    new AppRunner<Program>().UseCommandLogger();

                public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
            }
            // end-snippet
        }

        private static readonly TestEnvironment TestEnvironment = new()
        {
            FrameworkDescription = ".NET 5.0.13",
            OSDescription = "Microsoft Windows 10.0.12345",
            MachineName = "my-machine",
            UserName = "my-machine\\username"
        };

        public static BashSnippet Default = new ("command_logger_default_no_directive", 
            Program_Default.Program.AppRunner.UseTestEnv(TestEnvironment), 
            "example.exe", "Add 1 1", 0,
            @"2");

        public static BashSnippet Default_Directive = new("command_logger_default_directive",
            Program_Default.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "[cmdlog] Add 1 1", 0,
            @"
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
2");

        public class Program_Include_MachineAndUser
        {
            public class Program
            {
                static int Main(string[] args) => AppRunner.Run(args);

                public static AppRunner AppRunner => new AppRunner<Program>()
                    // begin-snippet: command_logger_include_machine_and_user
                    .UseCommandLogger(
                        excludeSystemInfo: true,
                        includeMachineAndUser: true);
                // end-snippet

                public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
            }
        }

        public static BashSnippet Include_MachineAndUser = new("command_logger_include_machine_and_user_exe",
            Program_Include_MachineAndUser.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "[cmdlog] Add 1 1", 0,
            @"
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
2");

        public class Program_Include_AppConfig
        {
            public class Program
            {
                static int Main(string[] args) => AppRunner.Run(args);

                public static AppRunner AppRunner => new AppRunner<Program>()
                    // begin-snippet: command_logger_appconfig
                    .UseCommandLogger(includeAppConfig: true);
                // end-snippet

                public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
            }
        }

        public static BashSnippet Include_AppConfig = new("command_logger_appconfig_exe",
            Program_Include_AppConfig.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "[cmdlog] Add 1 1", 0,
            @"
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
2");

        public class Program_Custom_Attribute
        {
            // begin-snippet: command_logger_custom_attribute
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
            // end-snippet
        }

        public static BashSnippet CustomAttribute_Enabled = new("command_logger_custom_attribute_enabled",
            Program_Custom_Attribute.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "Add 1 1", 0,
            @"
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
2");

        public static BashSnippet CustomAttribute_Disabled = new("command_logger_custom_attribute_disabled",
            Program_Custom_Attribute.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "Subtract 1 1", 0,
            @"0");

        public class Program_Root_Option
        {
            // begin-snippet: command_logger_root_option
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
            // end-snippet
        }

        public static BashSnippet RootOption = new("command_logger_root_option_exe",
            Program_Root_Option.Program.AppRunner.UseTestEnv(TestEnvironment),
            "example.exe", "--logcmd Add 1 1", 0,
            @"
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
2");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}