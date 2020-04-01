using System.Diagnostics;
using CommandDotNet.Diagnostics;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DebugDirectiveTests : TestBase
    {
        public DebugDirectiveTests(ITestOutputHelper output) : base(output)
        {
            // skip waiting for debugger to connect
            DebugDirective.InTestHarness = true;
        }

        [Fact]
        public void Directives_CanBeDisabled()
        {
            new AppRunner<App>(TestAppSettings.TestDefault.Clone(s => s.DisableDirectives = true))
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[debug] Do",
                        Then =
                        {
                            ExitCode = 1, // method should have been called
                            ResultsContainsTexts = { "Unrecognized command or argument '[debug]'" }
                        }
                    });
        }

        [Fact]
        public void DebugDirective_PrintsProcessId_And_WaitsForDebuggerToStart()
        {
            var processId = Process.GetCurrentProcess().Id;
            new AppRunner<App>()
                .UseDebugDirective()
                .VerifyScenario(TestOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "[debug] Do",
                        Then =
                        {
                            ExitCode = 5, // method should have been called
                            ResultsContainsTexts = {$"Attach your debugger to process {processId} (dotnet)."}
                        }
                    });
        }

        public class App
        {
            public int Do()
            {
                return 5;
            }
        }
    }
}