using System.Threading.Tasks;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class AsyncTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AsyncTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("get2", 2)]
        [InlineData("get00", 0)]
        [InlineData("get3", 3)]
        [InlineData("get01", 0)]
        public void TestAsyncFunctionality(string commandName, int expectedCode)
        {
            var result = new AppRunner<App>()
                .RunInMem(new[] {commandName}, _testOutputHelper);

            result.ExitCode.Should().Be(expectedCode, $"command '{commandName}' is expected to return '{expectedCode}'" );
            
            // ensure the non-return methods were called
            result.TestOutputs.Get<bool>().Should().BeTrue();
        }

        public class App
        {
            private TestOutputs TestOutputs { get; set; }

            [Command(Description = "Invokes an async method and exits with return code 2", Name = "get2")]
            public async Task<int> Get2Async()
            {
                TestOutputs.Capture(true);
                return await Task.FromResult(2);
            }

            [Command(Description = "Invokes an async method and exits with return code 0", Name = "get00")]
            public async Task Get0Async()
            {
                TestOutputs.Capture(true);
                await Task.CompletedTask;
            }

            [Command(Description = "Invokes an async method and exits with return code 3", Name = "get3")]
            public Task<int> Get3Async()
            {
                TestOutputs.Capture(true);
                return Task.FromResult(3);
            }

            [Command(Description = "Invokes an async method and exits with return code 0", Name = "get01")]
            public Task GetAsync()
            {
                TestOutputs.Capture(true);
                return Task.CompletedTask;
            }
        }
    }
}