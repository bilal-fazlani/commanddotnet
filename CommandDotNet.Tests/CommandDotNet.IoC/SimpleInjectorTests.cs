using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.IoC.SimpleInjector;
using CommandDotNet.TestTools;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class SimpleInjectorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SimpleInjectorTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void ShouldWork()
        {
            var container = new Container();
            container.Register<App>();
            container.Register<ISomeService, SomeService>();

            new AppRunner<App>()
                .UseSimpleInjector(container)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<App>(Lifestyle.Scoped);
            container.Register<ISomeService, SomeService>(Lifestyle.Scoped);

            ISomeService svcBeforeRun;
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                svcBeforeRun = container.GetInstance<ISomeService>();
            }

            var testOutputs = new AppRunner<App>()
                .UseSimpleInjector(container, runInScope: ctx => AsyncScopedLifestyle.BeginScope(container))
                .RunInMem("Do", _testOutputHelper)
                .TestOutputs;

            var services = testOutputs.Get<App.Services>();

            services.FromCtor.Should().BeSameAs(services.FromInterceptor);
            services.FromCtor.Should().NotBeSameAs(svcBeforeRun);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var container = new Container();
            // SimpleInjector will implicit register App if ISomeService is registered

            Assert.Throws<ActivationException>(() =>
                new AppRunner<App>()
                    .UseSimpleInjector(container)
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("No registration for type SimpleInjectorTests.App could be found and an implicit registration could not be made.");
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