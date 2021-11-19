using System.Threading.Tasks;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class AsyncTests
    {
        public AsyncTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Theory]
        [InlineData("get2", 2)]
        [InlineData("get00", 0)]
        [InlineData("get3", 3)]
        [InlineData("get01", 0)]
        public void TestAsyncFunctionality(string commandName, int expectedCode)
        {
            var result = new AppRunner<App>()
                .TrackingInvocations()
                .RunInMem(new[] {commandName});

            result.ExitCode.Should().Be(expectedCode, $"command '{commandName}' is expected to return '{expectedCode}'" );

            // ensure the non-return methods were called
            result.CommandContext.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
        }

        private class App
        {
            [Command("get2", Description = "Invokes an async method and exits with return code 2")]
            public async Task<int> Get2Async()
            {
                return await ExitCodes.ValidationError;
            }

            [Command("get00", Description = "Invokes an async method and exits with return code 0")]
            public async Task Get0Async()
            {
                await Task.CompletedTask;
            }

            [Command("get3", Description = "Invokes an async method and exits with return code 3")]
            public Task<int> Get3Async()
            {
                return Task.FromResult(3);
            }

            [Command("get01", Description = "Invokes an async method and exits with return code 0")]
            public Task GetAsync()
            {
                return Task.CompletedTask;
            }
        }
    }
}