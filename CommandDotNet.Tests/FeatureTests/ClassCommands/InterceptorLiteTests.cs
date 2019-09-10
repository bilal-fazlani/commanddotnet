using System;
using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorLiteTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InterceptorLiteTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InterceptorLiteMethodWithNoOptionsIsDetectedAndUsed()
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
        public void InterceptorLiteMethodWithOptionsIsDetectedAndUsed()
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
        public void InterceptorLiteMethodCanBypassNextDelegate()
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
        public void VoidInterceptorThrowsDescriptiveException()
        {
            Action testCode = () => new AppRunner<VoidInterceptor>().RunInMem("Do", _testOutputHelper);

            Assert.Throws<InvalidConfigurationException>(testCode)
                .Message.Should().Contain(
                    "`CommandDotNet.Tests.FeatureTests.ClassCommands.InterceptorLiteTests+VoidInterceptor.Intercept` " +
                    "must return type of System.Threading.Tasks.Task`1[System.Int32]");
        }

        class AppNoOptions
        {
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Intercept(Func<Task<int>> next)
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

            public Task<int> Intercept(Func<Task<int>> next, [Option]string stringOpt, [Option]bool skipCmd = false)
            {
                TestOutputs.Capture(true);
                TestOutputs.CaptureIfNotNull(stringOpt);
                return skipCmd 
                    ? Task.FromResult(0) 
                    : next();
            }

            public void Do(int arg1)
            {
                TestOutputs.Capture(arg1);
            }
        }

        class VoidInterceptor
        {
            public void Intercept(Func<Task<int>> next)
            {
                next().Wait();
            }

            public void Do()
            {
            }
        }
    }
}