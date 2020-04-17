using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class OnRunCompletedTests
    {
        private readonly ITestOutputHelper _output;

        public OnRunCompletedTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void OnRunCompleted_IsCalled_AfterPipelineIsExecuted()
        {
            bool wasCalled = false;
            new AppRunner<App>()
                .Configure(b => b.OnRunCompleted += args =>
                {
                    wasCalled = true;
                })
                .RunInMem("", _output);

            wasCalled.Should().BeTrue();
        }

        public class App
        {
            public void Do() { }
        }
    }
}