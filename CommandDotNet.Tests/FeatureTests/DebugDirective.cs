using System.Diagnostics;
using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DebugDirective : TestBase
    {
        public DebugDirective(ITestOutputHelper output) : base(output)
        {
            // skip waiting for debugger to connect
            Directives.InTestHarness = true;
        }

        [Fact]
        public void DebugDirective_PrintsProcessId_And_WaitsForDebuggerToStart()
        {
            var processId = Process.GetCurrentProcess().Id;
            Verify(new Given<App>
            {
                WhenArgs = "[debug] Do",
                Then =
                {
                    ExitCode = 5, // method should have been called
                    ResultsContainsTexts = { $"Attach your debugger to process {processId} (dotnet)." }
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