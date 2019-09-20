using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class OnRunCompletedTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public OnRunCompletedTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
                .RunInMem("", _testOutputHelper);

            wasCalled.Should().BeTrue();
        }

        public class App
        {
            public void Do() { }
        }
    }
}