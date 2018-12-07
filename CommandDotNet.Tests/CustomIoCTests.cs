using System;
using Autofac;
using CommandDotNet.Attributes;
using CommandDotNet.CommandInvoker;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class PrePostHookTests : TestBase
    {
        public PrePostHookTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void BothHooksFire()
        {
            var appRunner = new AppRunner<PrePostHookApp>(new AppSettings{EnableCommandHooks = true});
                
            appRunner.Run("Get", "--pre", "3", "--post", "5").Should().Be(0);
            
            Assert.True(PrePostHookApp.GetResults().PostHookFired);
        }
        
        [Fact]
        public void OptionArgumentsAreNotRequired()
        {
            var appRunner = new AppRunner<PrePostHookApp>(new AppSettings{EnableCommandHooks = true});
                
            appRunner.Run("Get", "--pre", "3", "--post", "5", "-o", "10").Should().Be(0);
            
            Assert.True(PrePostHookApp.GetResults().PostHookFired);
        }

        public class Results
        {
            public bool PreHookFired = false;
            public bool PostHookFired = false;
        }
        public class PrePostHookApp
        {
            private static Results _results = new Results();

            public static Results GetResults()
            {
                var results = _results;
                _results = new Results();
                return results;
            }
            
            [PreHook]
            public void PreHook(PreHookModel preHookModel)
            {
                Assert.NotNull(preHookModel);
                _results.PreHookFired = true;
            }
            
            [PostHook]
            public void PostHook(PostHookModel postHookModel, OptionalModel optionalModel)
            {
                Assert.NotNull(postHookModel);
                _results.PostHookFired = true;
            }
        
            public void Get(PreHookModel preHookModel, PostHookModel postHookModel, OptionalModel commonHookModel)
            {
                Assert.True(_results.PreHookFired);
                Assert.False(_results.PostHookFired);
            }
        }
        
        public class PreHookModel : IArgumentModel
        {
            [Option(LongName = "pre")]
            public int Value { get; set; }
        }
        
        public class PostHookModel : IArgumentModel
        {
            [Option(LongName = "post")]
            public int Value { get; set; }
        }
        
        public class OptionalModel : IArgumentModel
        {
            [Option(ShortName = "o")]
            public int Value { get; set; }
        }
    }
    
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