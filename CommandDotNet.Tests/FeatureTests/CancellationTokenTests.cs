using System;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CancellationTokenTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CancellationTokenTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void MiddlewarePipelineShouldStopWhenCancellationIsRequested()
        {
            var tokenSource = new CancellationTokenSource();
            new AppRunner<App>()
                .Configure(c =>
                {
                    c.Services.Set(tokenSource);
                    c.CancellationToken = tokenSource.Token;
                    c.UseMiddleware(Cancel, MiddlewareStages.PostTransformTokensPreBuild);
                    c.UseMiddleware(Throw, MiddlewareStages.PostTransformTokensPreBuild);
                })
                .RunInMem(new string[0], _testOutputHelper)
                .ConsoleAllOutput.Should().BeEmpty();
        }

        [Fact]
        public void VerifyThrowThrows()
        {
            // this test ensures the previous test is not passing with a false positive
            var tokenSource = new CancellationTokenSource();
            var appRunner = new AppRunner<App>()
                .Configure(c =>
                {
                    c.Services.Set(tokenSource);
                    c.CancellationToken = tokenSource.Token;
                    c.UseMiddleware(Throw, MiddlewareStages.PostTransformTokensPreBuild);
                });

            Assert.Throws<Exception>(() => appRunner.RunInMem(new string[0], _testOutputHelper))
                .Message.Should().Be("This middleware should not have been called");
        }

        private Task<int> Cancel(CommandContext context, Func<CommandContext, Task<int>> next)
        {
            context.AppConfig.Services.Get<CancellationTokenSource>().Cancel();
            return next(context);
        }

        private Task<int> Throw(CommandContext context, Func<CommandContext, Task<int>> next)
        {
            throw new Exception("This middleware should not have been called");
        }

        public class App
        {
            public void Do()
            {

            }
        }
    }
}