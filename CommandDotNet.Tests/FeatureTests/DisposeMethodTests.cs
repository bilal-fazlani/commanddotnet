using System;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{

    public class DisposeMethodTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public DisposeMethodTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void When_IDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Scenario<DisposableApp>
            {
                Given = {AppSettings = BasicHelp},
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_IDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Scenario<DisposableApp>
            {
                Given = {AppSettings = DetailedHelp},
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_NotIDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Scenario<NotDisposableApp>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:
  Dispose  " } }
            });
        }

        [Fact]
        public void When_NotIDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Scenario<NotDisposableApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:

  Dispose  " } }
            });
        }

        [Fact]
        public void When_IDisposable_CallsDisposeMethod()
        {
            Verify(new Scenario<DisposableApp>
            {
                WhenArgs = "Do",
                Then = {Outputs = {true}}
            });
        }

        [Fact]
        public void When_NotIDisposable_CallsDisposeMethod()
        {
            Verify(new Scenario<NotDisposableApp>
            {
                WhenArgs = "Dispose",
                Then = { Outputs = { true } }
            });
        }

        public class DisposableApp : IDisposable
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do()
            {
            }

            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }

        public class NotDisposableApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }

    }
}