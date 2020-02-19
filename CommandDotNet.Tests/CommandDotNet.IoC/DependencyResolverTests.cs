using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using CommandDotNet.Execution;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.IoC.SimpleInjector;
using CommandDotNet.TestTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class DependencyResolverTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DependencyResolverTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void MicrosoftDependencyInjection_ShouldWork()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();

            new AppRunner<App>()
                .UseMicrosoftDependencyInjection(serviceProvider)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void MicrosoftDependencyInjection_CanSpecifyScope()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();

            new AppRunner<App>()
                .UseMicrosoftDependencyInjection(serviceProvider, runInScope: ctx => serviceProvider.CreateScope())
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void MicrosoftDependencyInjection_WhenUnregisteredType_ShouldThrowException()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider(skipApp: true);

            Assert.Throws<InvalidOperationException>(() =>
                new AppRunner<App>()
                    .UseMicrosoftDependencyInjection(serviceProvider)
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("No service for type 'CommandDotNet.Tests.CommandDotNet.IoC.DependencyResolverTests+App'");
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

        [Fact]
        public void Autofac_ShouldWork()
        {
            var container = ConfigureAutofacContainer();

            new AppRunner<App>()
                .UseAutofac(container)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void Autofac_CanSpecifyScope()
        {
            var container = ConfigureAutofacContainer();

            new AppRunner<App>()
                .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope())
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void Autofac_WhenUnregisteredType_ShouldThrowException()
        {
            var container = ConfigureAutofacContainer(skipApp: true);

            Assert.Throws<ComponentNotRegisteredException>(() =>
                new AppRunner<App>()
                    .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope())
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("The requested service 'CommandDotNet.Tests.CommandDotNet.IoC.DependencyResolverTests+App'");

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

        [Fact]
        public void SimpleInjector_ShouldWork()
        {
            var container = new Container();
            container.Register<App>();
            container.Register<ISomeService, SomeService>();

            new AppRunner<App>()
                .UseSimpleInjector(container)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void SimpleInjector_CanSpecifyScope()
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
        public void SimpleInjector_WhenUnregisteredType_ShouldThrowException()
        {
            var container = new Container();
            // SimpleInjector will implicit register App if ISomeService is registered

            Assert.Throws<ActivationException>(() =>
                new AppRunner<App>()
                    .UseSimpleInjector(container)
                    .RunInMem("Do", _testOutputHelper)
            ).Message.Should().StartWith("No registration for type DependencyResolverTests.App could be found and an implicit registration could not be made.");
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