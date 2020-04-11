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
            new AppRunner<DisposableApp>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = {OutputNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_IDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<DisposableApp>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = {OutputNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_NotIDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = { OutputContainsTexts = { @"Commands:
  Dispose  " } }
            });
        }

        [Fact]
        public void When_NotIDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "-h",
                Then = { OutputContainsTexts = { @"Commands:

  Dispose  " } }
            });
        }

        [Fact]
        public void When_IDisposable_CallsDisposeMethod()
        {
            new AppRunner<DisposableApp>().Verify(_output, new Scenario
            {
                WhenArgs = "Do",
                Then = {Captured = {true}}
            });
        }

        [Fact]
        public void When_NotIDisposable_CallsDisposeMethod()
        {
            new AppRunner<NotDisposableApp>().Verify(_output, new Scenario
            {
                WhenArgs = "Dispose",
                Then = { Captured = { true } }
            });
        }

        private class DisposableApp : IDisposable
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do()
            {
            }

            public void Dispose()
            {
                TestCaptures.Capture(true);
            }
        }

        private class NotDisposableApp
        {
            private TestCaptures TestCaptures { get; set; }

            // use the name Dispose to prove it can be a command name
            // and that the Dispose name is filtered out only for IDisposable's
            public void Dispose()
            {
                TestCaptures.Capture(true);
            }
        }

    }
}