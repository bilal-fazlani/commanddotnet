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
                    c.Services.AddOrUpdate(tokenSource);
                    c.CancellationToken = tokenSource.Token;
                    c.UseMiddleware(Cancel, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(Throw, MiddlewareStages.PostTokenizePreParseInput);
                })
                .RunInMem(new string[0], _testOutputHelper)
                .ConsoleOutAndError.Should().BeEmpty();
        }

        [Fact]
        public void EnsureNoFalsePositives()
        {
            // this test ensures the previous test is not passing with a false positive
            var tokenSource = new CancellationTokenSource();
            var appRunner = new AppRunner<App>()
                .Configure(c =>
                {
                    c.Services.AddOrUpdate(tokenSource);
                    c.CancellationToken = tokenSource.Token;
                    c.UseMiddleware(Throw, MiddlewareStages.PostTokenizePreParseInput);
                });

            Assert.Throws<Exception>(() => appRunner.RunInMem(new string[0], _testOutputHelper))
                .Message.Should().Be("This middleware should not have been called");
        }

        private Task<int> Cancel(CommandContext context, ExecutionDelegate next)
        {
            context.AppConfig.Services.Get<CancellationTokenSource>().Cancel();
            return next(context);
        }

        private Task<int> Throw(CommandContext context, ExecutionDelegate next)
        {
            throw new Exception("This middleware should not have been called");
        }

        public class App
        {
            public void Do() { }
        }
    }
}