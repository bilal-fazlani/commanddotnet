using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CachedArgumentModelResolverTests : TestBase
    {
        public CachedArgumentModelResolverTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        
        /*
         * 1 - argument model is injected w/o DI
         * 2 - argument model is injected w/ DI - not registered
         * 3 - argument model is injected w/ DI - registered
         * 4 - parameter options still work with all 3 scenarios
         */

        [Fact]
        public void WhenNoDiArgumentModelsAreInjected()
        {
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>();
                
            appRunner.Run("TestInjection").Should().Be(0);
        }

        [Fact]
        public void WhenNoDiInjectedModelsAreSameInstanceAsParams()
        {
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>();
                
            appRunner.Run("SameInstance", "-v", "1", "-w", "1").Should().Be(2);
        }

        [Fact]
        public void WhenNotRegisteredWithDiArgumentModelsAreInjected()
        {
            var resolver = new CustomDependencyResolver(registerModel: false);
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>()
                .UseDependencyResolver(resolver);
                
            appRunner.Run("TestInjection").Should().Be(0);
            resolver.ResolveRequests.Count.Should().Be(1);
            resolver.ResolveRequests.Contains(typeof(Model));
        }

        [Fact]
        public void WhenNotRegisteredWithDiInjectedModelsAreSameInstanceAsParams()
        {
            var resolver = new CustomDependencyResolver(registerModel: false);
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>()
                .UseDependencyResolver(resolver);
                
            appRunner.Run("SameInstance", "-v", "1", "-w", "1").Should().Be(2);
            resolver.ResolveRequests.Count.Should().Be(1);
            resolver.ResolveRequests.Contains(typeof(Model));
        }

        [Fact]
        public void WhenRegisteredWithDiArgumentModelsAreInjected()
        {
            var resolver = new CustomDependencyResolver(registerModel: true);
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>()
                .UseDependencyResolver(resolver);
                
            appRunner.Run("TestInjection").Should().Be(0);
            resolver.ResolveRequests.Count.Should().Be(1);
            resolver.ResolveRequests.Contains(typeof(Model));
        }

        [Fact]
        public void WhenRegisteredWithDiInjectedModelsAreSameInstanceAsParams()
        {
            var resolver = new CustomDependencyResolver(registerModel: true);
            var appRunner = new AppRunner<CachedArgumentModelResolverApp>()
                .UseDependencyResolver(resolver);
                
            appRunner.Run("SameInstance", "-v", "1", "-w", "1").Should().Be(2);
            resolver.ResolveRequests.Count.Should().Be(1);
            resolver.ResolveRequests.Contains(typeof(Model));
        }
        
        public class CachedArgumentModelResolverApp
        {
            [InjectProperty]
            public Model Model { get; set; }
        
            public void TestInjection()
            {
                if(Model == null)
                    throw new Exception("dependency was not injected");
            }
            
            public int SameInstance(Model model, [Option(ShortName = "w")]int otherValue)
            {
                if(!ReferenceEquals(Model, model))
                    throw new Exception("injected dependency is different instance than command param");
                return model.Value + otherValue;
            }
        }

        public class Model : IArgumentModel
        {
            [Option(ShortName = "v")]
            public int Value { get; set; }
        }
        
        public class CustomDependencyResolver : IDependencyResolver
        {
            private IContainer _container;
            
            public List<Type> ResolveRequests = new List<Type>();
        
            public CustomDependencyResolver(bool registerModel)
            {
                ContainerBuilder containerBuilder = new ContainerBuilder();

                if (registerModel)
                {
                    containerBuilder.RegisterInstance(new Model());
                }
            
                _container = containerBuilder.Build();
            }
        
            public object Resolve(Type type)
            {
                this.ResolveRequests.Add(type);
                return _container.Resolve(type);
            }
        }
    }
}