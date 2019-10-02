using System;
using Autofac;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.IoC.SimpleInjector;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
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
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ISomeService, SomeService>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            new AppRunner<App>()
                .UseMicrosoftDependencyInjection(serviceProvider)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void Autofac_ShouldWork()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<SomeService>().As<ISomeService>();
            IContainer container = containerBuilder.Build();

            new AppRunner<App>()
                .UseAutofac(container)
                .RunInMem("Do", _testOutputHelper);
        }

        [Fact]
        public void SimpleInjector_ShouldWork()
        {
            var container = new Container();
            container.Register<ISomeService, SomeService>();

            new AppRunner<App>()
                .UseSimpleInjector(container)
                .RunInMem("Do", _testOutputHelper);
        }

        class App
        {
            private readonly ISomeService _someService;

            public App(ISomeService someService)
            {
                _someService = someService;
            }

            public void Do()
            {
                if(_someService == null)
                    throw new Exception("SomeService was not injected");
            }
        }

        public interface ISomeService { }

        public class SomeService : ISomeService { }
    }
}