using System;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{

    public class DisposeMethod : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public DisposeMethod(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BasicHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "Do -h",
                Then = { ResultsNotContainsTexts = { "Dispose" } }
            });
        }

        [Fact]
        public void DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = DetailedHelp},
                WhenArgs = "Do -h",
                Then = {ResultsNotContainsTexts = { "Dispose" } }
            });
        }

        [Fact]
        public void CallsDisposeMethod()
        {
            Verify(new Given<App>
            {
                WhenArgs = "Do",
                Then =
                {
                    Outputs = { true }
                }
            });
        }

        public class App : IDisposable
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do()
            {
            }

            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }
    }
}