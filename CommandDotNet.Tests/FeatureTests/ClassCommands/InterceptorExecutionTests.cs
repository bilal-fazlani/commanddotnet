using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InterceptorExecutionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Parent_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [command]

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:

  arg1  <NUMBER>"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Parent_ContainsInterceptorOptions()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --stringOpt      <TEXT>

  --skipCmd

  --useReturnCode  <NUMBER>

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:

  arg1  <NUMBER>"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do 1",
                    Then =
                    {
                        Outputs = { true, 1 }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--stringOpt lala Do 1",
                    Then =
                    {
                        Outputs =
                        {
                            new AppWithInteceptorOptions.InterceptOptions {stringOpt = "lala"},
                            1
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_CanBypassNextDelegate()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = " --skipCmd Do 1",
                    Then =
                    {
                        // does not contain output from Do method
                        Outputs = { new AppWithInteceptorOptions.InterceptOptions{skipCmd = true} }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_CanChangeReturnCode()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = " --useReturnCode 5 Do 1",
                    Then =
                    {
                        ExitCode = 5,
                        Outputs =
                        {
                            new AppWithInteceptorOptions.InterceptOptions {useReturnCode = 5},
                            1
                        }
                    }
                });
        }

        class AppWithNoInterceptorOptions
        {
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next)
            {
                TestOutputs.Capture(true);
                return next();
            }

            public void Do(int arg1)
            {
                TestOutputs.Capture(arg1);
            }
        }

        class AppWithInteceptorOptions
        {
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next,
                InterceptOptions interceptOptions)
            {
                TestOutputs.Capture(interceptOptions);
                if (interceptOptions.skipCmd)
                {
                    return Task.FromResult(0);
                }

                var returnCode = next();
                return interceptOptions.useReturnCode.HasValue
                    ? Task.FromResult(interceptOptions.useReturnCode.Value)
                    : returnCode;
            }

            public void Do(int arg1)
            {
                TestOutputs.Capture(arg1);
            }

            public class InterceptOptions : IArgumentModel
            {
                public string stringOpt { get; set; }
                public bool skipCmd { get; set; } = false;
                public int? useReturnCode { get; set; }
            }
        }
    }
}