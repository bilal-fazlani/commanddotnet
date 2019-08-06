using System.Diagnostics;
using CommandDotNet.Directives;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DebugDirective : TestBase
    {
        private readonly AppSettings DirectivesEnabled = TestAppSettings.TestDefault.Clone(s => s.EnableDirectives = true);

        public DebugDirective(ITestOutputHelper output) : base(output)
        {
            // skip waiting for debugger to connect
            Directives.DebugDirective.InTestHarness = true;
        }

        [Fact]
        public void Directives_DisabledByDefault()
        {
            new AppRunner<App>()
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
            new AppRunner<App>(DirectivesEnabled)
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