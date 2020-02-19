using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using CommandDotNet.Execution;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class AutofacTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AutofacTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldWork()
        {
            var container = ConfigureAutofacContainer();

            new AppRunner<App>()
                .UseAutofac(container)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var container = ConfigureAutofacContainer();

            new AppRunner<App>()
                .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope())
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var container = ConfigureAutofacContainer(skipApp: true);

            Assert.Throws<ComponentNotRegisteredException>(() =>
                new AppRunner<App>()
                    .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope())
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("The requested service 'CommandDotNet.Tests.CommandDotNet.IoC.AutofacTests+App'");

        }

        private static IContainer ConfigureAutofacContainer(bool skipApp = false)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            if (!skipApp)
            {
                containerBuilder.RegisterType<App>();
            }
            containerBuilder.RegisterType<SomeService>().As<ISomeService>();
            IContainer container = containerBuilder.Build();
            return container;
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