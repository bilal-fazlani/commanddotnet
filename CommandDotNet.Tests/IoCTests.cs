using Autofac;
using CommandDotNet.Attributes;
using CommandDotNet.IoC.Autofac;
using FluentAssertions;
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
        public void CanResolveDependencyInNestedCommand()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<Service>().As<IService>();
            IContainer container = containerBuilder.Build();

            AppRunner<NestedCommandApp> appRunner = new AppRunner<NestedCommandApp>().UseAutofac(container);
                
            appRunner.Run("InnerApp", "-a", "3", "Process").Should().Be(7);
        }
    }

    public class NestedCommandApp
    {
        public class InnerApp
        {
            private readonly int _additionFactor;
            public IService Service { get; set; }

            public InnerApp([Option(ShortName = "a")]int additionFactor)
            {
                _additionFactor = additionFactor;
            }
            
            public int Process()
            {
                return Service.GetValue() + _additionFactor;
            }
        }
    }

    public interface IService
    {
        int GetValue();
    }

    public class Service : IService
    {
        public int GetValue()
        {
            return 4;
        }
    }
}