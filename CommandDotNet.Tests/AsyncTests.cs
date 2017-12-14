using System.Threading.Tasks;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class AsyncTests : TestBase
    {
        public AsyncTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("get2", 2)]
        [InlineData("get00", 0)]
        [InlineData("get3", 3)]
        [InlineData("get01", 0)]
        public void TestAsyncFunctionality(string commandName, int expectedCode)
        {
            AppRunner<AsyncTestApp> testApp = new AppRunner<AsyncTestApp>();
            int exitCode = testApp.Run(new[] {commandName});
            exitCode.Should().Be(expectedCode, $"command '{commandName}' is expected to return '{expectedCode}'" );
        }
    }
    
    public class AsyncTestApp
    {
        [ApplicationMetadata(Description = "Invokes an async method and exits with return code 2", Name = "get2")]
        public async Task<int> Get2Async()
        {
            return await Task.FromResult(2);
        }
        
        [ApplicationMetadata(Description = "Invokes an async method and exits with return code 0", Name = "get00")]
        public async Task Get0Async()
        {
            await Task.CompletedTask;
        }

        [ApplicationMetadata(Description = "Invokes an async method and exits with return code 3", Name = "get3")]
        public Task<int> Get3Async()
        {
            return Task.FromResult(3);
        }
        
        [ApplicationMetadata(Description = "Invokes an async method and exits with return code 0", Name = "get01")]
        public Task GetAsync()
        {
            return Task.CompletedTask;
        }
    }
}