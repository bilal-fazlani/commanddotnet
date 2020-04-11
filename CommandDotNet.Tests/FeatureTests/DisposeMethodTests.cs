using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DisposeMethodTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public DisposeMethodTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void When_IDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<DisposableApp>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_IDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<DisposableApp>(DetailedHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_NotIDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:
  Dispose  " } }
            });
        }

        [Fact]
        public void When_NotIDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(DetailedHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:

  Dispose  " } }
            });
        }

        [Fact]
        public void When_IDisposable_CallsDisposeMethod()
        {
            new AppRunner<DisposableApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Do",
                Then = {Outputs = {true}}
            });
        }

        [Fact]
        public void When_NotIDisposable_CallsDisposeMethod()
        {
            new AppRunner<NotDisposableApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Dispose",
                Then = { Outputs = { true } }
            });
        }

        private class DisposableApp : IDisposable
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

        private class NotDisposableApp
        {
            private TestOutputs TestOutputs { get; set; }

            // use the name Dispose to prove it can be a command name
            // and that the Dispose name is filtered out only for IDisposable's
            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }

    }
}