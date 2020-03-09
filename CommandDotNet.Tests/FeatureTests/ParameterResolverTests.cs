using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParameterResolverTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ParameterResolverTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void ParameterServices_AreNotIncludedInBasicHelp()
        {
            new AppRunner<App>(TestAppSettings.BasicHelp)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  intOperand

Options:
  --stringOption" }
                });
        }

        [Fact]
        public void ParameterServices_AreNotIncludedInDetailedHelp()
        {
            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  intOperand  <NUMBER>

Options:

  --stringOption  <TEXT>" }
                });
        }

        [Fact]
        public void ParameterServices_ArePassedToCommandAndInterceptorMethod()
        {
            new AppRunner<App>()
                .Configure(c => c.CancellationToken = new CancellationTokenSource().Token)
                .VerifyScenario(_testOutputHelper, new Scenario
            {
                WhenArgs = "Do 7 --stringOption optValue",
                Then =
                {
                    AllowUnspecifiedOutputs = true,
                    Outputs =
                    {
                        new DoResults
                        {
                            IntOperand = 7,
                            StringOption = "optValue",
                            ParameterServices =
                            {
                                CommandContextIsNull = false,
                                ConsoleIsNull = false,
                                CancellationTokenIsNone = false
                            }
                        },
                        new InterceptorResults
                        {
                            ParameterServices =
                            {
                                CommandContextIsNull = false,
                                ConsoleIsNull = false,
                                CancellationTokenIsNone = false
                            }
                        }
                    }
                }
            });
        }

        [Fact]
        public void ExternalParameterService_WhenNotRegistered_ResultContainsActionableErrorMessage()
        {
            new AppRunner<SomeServiceApp>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts =
                        {
                            "CommandDotNet.Tests.FeatureTests.ParameterResolverTests+SomeService is not supported.",
                            "If it is a service and not an argument, register using AppRunner.Configure(b => b.UseParameterResolver(ctx => ...)); "
                        }
                    }
                });
        }

        [Fact]
        public void ExternalParameterService_CanBeRegistered()
        {
            var someSvc = new SomeService();
            new AppRunner<SomeServiceApp>()
                .Configure(b => b.UseParameterResolver(ctx => someSvc))
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do",
                    Then =
                    {
                        Outputs =
                        {
                            new DoResults()
                            {
                                ParameterServices =
                                {
                                    SomeService = someSvc
                                }
                            }
                        }
                    }
                });
        }

        public class SomeServiceApp
        {
            TestOutputs TestOutputs { get; set; }

            public void Do(SomeService someService, [Operand] int intOperand, [Option] string stringOption = null)
            {
                TestOutputs.Capture(new DoResults
                {
                    IntOperand = intOperand,
                    StringOption = stringOption,
                    ParameterServices =
                    {
                        SomeService = someService
                    }
                });
            }
        }

        public class App
        {
            TestOutputs TestOutputs { get; set; }

            public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext commandContext, IConsole console, CancellationToken cancellationToken)
            {
                TestOutputs.Capture(new InterceptorResults
                {
                    ParameterServices =
                    {
                        CommandContextIsNull = commandContext == null,
                        ConsoleIsNull = console == null,
                        CancellationTokenIsNone = cancellationToken == CancellationToken.None,
                    }
                });
                return next();
            }

            public void Do(CommandContext commandContext, IConsole console, CancellationToken cancellationToken, [Operand] int intOperand, [Option] string stringOption = null)
            {
                TestOutputs.Capture(new DoResults
                {
                    IntOperand = intOperand,
                    StringOption = stringOption,
                    ParameterServices =
                    {
                        CommandContextIsNull = commandContext == null,
                        ConsoleIsNull = console == null,
                        CancellationTokenIsNone = cancellationToken == CancellationToken.None,
                    }
                });
            }
        }

        public class SomeService
        {

        }

        public class ParameterServices
        {
            public bool CommandContextIsNull;
            public bool ConsoleIsNull;
            public bool CancellationTokenIsNone;
            public SomeService SomeService;
        }

        public class DoResults
        {
            public int IntOperand;
            public string StringOption;
            public ParameterServices ParameterServices = new ParameterServices();
        }

        public class InterceptorResults
        {
            public ParameterServices ParameterServices = new ParameterServices();
        }
    }
}