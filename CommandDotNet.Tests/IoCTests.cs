using System;
using Autofac;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class IoCTests : TestBase
    {
        public IoCTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanAutofacInjectService()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Service>().As<IService>();

            IContainer container = containerBuilder.Build();

            AppRunner<ServiceApp> serviceApp = new AppRunner<ServiceApp>().UseAutofac(container);
            
            serviceApp.Run("Process").Should().Be(4);
        }
        
        [Fact]
        public void CanMicrosfotInjectService()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, Service>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            AppRunner<ServiceApp> serviceApp =
                new AppRunner<ServiceApp>().UseMicrosoftDependencyInjection(serviceProvider);
            
            serviceApp.Run("Process").Should().Be(4);
        }
    }

    public class ServiceApp
    {
        public IService Service { get; set; }

        public int Process()
        {
            return Service?.value ?? throw new Exception("Service is not injected");
        }
    }

    public interface IService
    {
        int value { get; set; }
    }

    public class Service : IService
    {
        public int value { get; set; } = 4;
    }
}