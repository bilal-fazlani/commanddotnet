using System;
using System.Text;
using System.Threading.Tasks;
using CommandDotNet.CommandLogger;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.CommandLogger
{
    public class CommandLoggerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CommandLoggerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void PasswordsAreObscured()
        {
            new AppRunner<App>()
                .UseCommandLogger()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--password super-secret Do lala",
                    Then =
                    {
                        Result = $@"
***************************************
Original input:
  --password ***** Do lala

command: Do

arguments:

  textOperand <Text>
    value: lala
    inputs: lala
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value: *****
    inputs: ***** (from: --password *****)
    default:
***************************************"
                    }
                });
        }

        [Fact]
        public void SystemInfo_CanBe_Shown()
        {
            new AppRunner<App>()
                .UseCommandLogger(includeSystemInfo: true)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = $@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:

Tool version  = testhost.dll 16.2.0
.Net version  = {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim()}
OS version    = {System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim()}
Machine       = {Environment.MachineName}
Username      = {Environment.UserDomainName}\{Environment.UserName}
***************************************"
                    }
                });
        }

        [Fact]
        public void AppConfig_CanBe_Shown()
        {
            new AppRunner<App>()
                .UseCommandLogger(includeAppConfig:true)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = $@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:

AppConfig:
  AppSettings:
    AllowArgumentSeparator: False
    ArgumentTypeDescriptors: ArgumentTypeDescriptors:
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.BoolTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.EnumTypeDescriptor
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Character'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Decimal'
      ErrorReportingDescriptor > DelegatedTypeDescriptor: 'Double'
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.ComponentModelTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.StringCtorTypeDescriptor
    BooleanMode: Implicit
    DefaultArgumentMode: Operand
    DisableDirectives: False
    GuaranteeOperandOrderInArgumentModels: False
    Help: CommandDotNet.Help.AppHelpSettings
    HelpTextStyle: Detailed
    IgnoreUnexpectedOperands: False
    MethodArgumentMode: Operand
    ThrowOnUnexpectedArgument: True
  DependencyResolver:
  HelpProvider: CommandDotNet.Help.HelpTextProvider
  TokenTransformations:
    expand-clubbed-flags(2147483647)
    split-option-assignments(2147483647)
  MiddlewarePipeline:
    TokenizerPipeline.TokenizeInputMiddleware
    ClassModelingMiddleware.CreateRootCommand
    CommandParser.ParseInputMiddleware
    ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
    HelpMiddleware.DisplayHelp
    BindValuesMiddleware.BindValues
    ResolveCommandClassesMiddleware.ResolveCommandClassInstances
    CommandLoggerMiddleware.CommandLogger
    AppRunnerTestExtensions.InjectTestOutputs
    ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
  ParameterResolvers:
    CommandDotNet.CommandContext
    CommandDotNet.Rendering.IConsole
    System.Threading.CancellationToken
***************************************"
                    }
                });
        }

        [Fact]
        public void AdditionalInfo_CanBe_Null()
        {
            new AppRunner<App>()
                .UseCommandLogger(additionalInfoCallback: ctx => null)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = $@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:
***************************************"
                    }
                });
        }

        [Fact]
        public void AdditionalInfo_CanBe_Shown()
        {
            new AppRunner<App>()
                .UseCommandLogger(additionalInfoCallback: ctx => new (string, string)[]
                {
                    ("header1", "value1" ),
                    ("header2", "value2" ),

                })
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = $@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:

header1  = value1
header2  = value2
***************************************"
                    }
                });
        }

        [Fact]
        public void WriterFactory_Can_ReturnNull_And_NothingIsLogged()
        {
            var sb = new StringBuilder();

            new AppRunner<App>()
                .UseCommandLogger(writerFactory: context => null)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = ""
                    }
                });
        }

        [Fact]
        public void WriterFactory_CanBe_Provided()
        {
            var sb = new StringBuilder();

            new AppRunner<App>()
                .UseCommandLogger(writerFactory: context => text => sb.Append(text))
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Result = ""
                    }
                });

            sb.ToString().Should().Be(@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value: 
    inputs:
    default:

options:

  textOption <Text>
    value: 
    inputs:
    default:

  password <Text>
    value: 
    inputs:
    default:
***************************************
");
        }

        public class App
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, Password password)
            {
                return next();
            }

            public void Do(
                [Option] string textOption,
                [Operand] string textOperand)
            {
            }
        }
    }
}