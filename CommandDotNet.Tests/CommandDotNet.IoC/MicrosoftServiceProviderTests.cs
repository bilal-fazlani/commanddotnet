using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.TestTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class MicrosoftServiceProviderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MicrosoftServiceProviderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldWork()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();

            new AppRunner<App>()
                .UseMicrosoftDependencyInjection(serviceProvider)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();

            new AppRunner<App>()
                .UseMicrosoftDependencyInjection(serviceProvider, runInScope: ctx => serviceProvider.CreateScope())
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider(skipApp: true);

            Assert.Throws<InvalidOperationException>(() =>
                new AppRunner<App>()
                    .UseMicrosoftDependencyInjection(serviceProvider)
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("No service for type 'CommandDotNet.Tests.CommandDotNet.IoC.MicrosoftServiceProviderTests+App'");
        }

        private static IServiceProvider ConfigureMicrosoftServiceProvider(bool skipApp = false)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            if (!skipApp)
            {
                serviceCollection.AddScoped<App>();
            }
            serviceCollection.AddScoped<ISomeService, SomeService>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        class App
        {
            private readonly ISomeService _someService;
            private TestOutputs TestOutputs { get; set; }

            public App(ISomeService someService)
            {
                _someService = someService;
            }

            public Task<int> Intercept(CommandContext context, ExecutionDelegate next)
            {
                TestOutputs.Capture(new Services
                {
                    FromCtor = _someService,
                    FromInterceptor = (ISomeService)context.AppConfig.DependencyResolver.Resolve(typeof(ISomeService))
                });
                return next(context);
            }

            public void Do()
            {
                if(_someService == null)
                    throw new Exception("SomeService was not injected");
            }

            public class Services
            {
                public ISomeService FromCtor { get; set; }
                public ISomeService FromInterceptor { get; set; }
            }
        }

        public interface ISomeService { }

        public class SomeService : ISomeService { }
    }
}