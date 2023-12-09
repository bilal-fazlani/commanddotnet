using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class OnRunCompletedTests
    {
        public OnRunCompletedTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void OnRunCompleted_IsCalled_AfterPipelineIsExecuted()
        {
            bool wasCalled = false;
            new AppRunner<App>()
                .Configure(b => b.OnRunCompleted += args =>
                {
                    // ensure OnRunCompleted waits for the pipeline to complete
                    App.Executed.Should().BeTrue();
                    wasCalled = true;
                })
                .RunInMem("Do");

            wasCalled.Should().BeTrue();
        }

        private class App
        {
            public static bool Executed { get; set; }

            public async Task Do()
            {
                await Task.Delay(1000);
                Executed = true;
            }
        }
    }
}