using System;
using Autofac;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CustomIoCTests : TestBase
    {
        public CustomIoCTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void TestCustomDependencyResolverFeature()
        {
            AppRunner<CustomIoCApp> appRunner = new AppRunner<CustomIoCApp>()
                .UseDependencyResolver(new CustomDependencyResolver());
                
            appRunner.Run("Get").Should().Be(0);
        }
    }

    public class CustomIoCApp
    {
        [InjectProperty]
        public CustomDependency CustomDependency { get; set; }
        
        public void Get()
        {
            if(CustomDependency == null)
                throw new Exception("dependency was not injected");
        }
    }

    public class CustomDependency
    {
        
    }

    public class CustomDependencyResolver : IDependencyResolver
    {
        private IContainer _container;
        
        public CustomDependencyResolver()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<CustomDependency>();
            
            _container = containerBuilder.Build();
        }
        
        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}