using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
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
        public void InterceptorExecutionMethodWithNoOptionsIsDetectedAndUsed()
        {
            new AppRunner<AppNoOptions>()
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
        public void InterceptorExecutionMethodWithOptionsIsDetectedAndUsed()
        {
            new AppRunner<AppWithOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--stringOpt lala Do 1",
                    Then =
                    {
                        Outputs = { true, "lala", 1 }
                    }
                });
        }

        [Fact]
        public void InterceptorExecutionMethodCanBypassNextDelegate()
        {
            new AppRunner<AppWithOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = " --skipCmd Do 1",
                    Then =
                    {
                        Outputs = { true }
                    }
                });
        }

        [Fact]
        public void InterceptorExecutionMethodCanChangeReturnCode()
        {
            new AppRunner<AppWithOptions>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = " --useReturnCode 5 Do 1",
                    Then =
                    {
                        ExitCode = 5,
                        Outputs = { true, 1 }
                    }
                });
        }

        [Fact]
        public void VoidInterceptorThrowsDescriptiveException()
        {
            Action testCode = () => new AppRunner<VoidInterceptor>().RunInMem("Do", _testOutputHelper);

            Assert.Throws<InvalidConfigurationException>(testCode)
                .Message.Should().Contain(
                    "`CommandDotNet.Tests.FeatureTests.ClassCommands.InterceptorExecutionTests+VoidInterceptor.Intercept` " +
                    "must return type of System.Threading.Tasks.Task`1[System.Int32]");
        }

        class AppNoOptions
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

        class AppWithOptions
        {
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next, 
                string stringOpt, 
                bool skipCmd = false,
                int? useReturnCode = null)
            {
                TestOutputs.Capture(true);
                TestOutputs.CaptureIfNotNull(stringOpt);
                if (skipCmd)
                {
                    return Task.FromResult(0);
                }

                var returnCode = next();
                return useReturnCode.HasValue
                    ? Task.FromResult(useReturnCode.Value)
                    : returnCode;
            }

            public void Do(int arg1)
            {
                TestOutputs.Capture(arg1);
            }
        }

        class VoidInterceptor
        {
            public void Intercept(InterceptorExecutionDelegate next)
            {
                next().Wait();
            }

            public void Do()
            {
            }
        }
    }
}