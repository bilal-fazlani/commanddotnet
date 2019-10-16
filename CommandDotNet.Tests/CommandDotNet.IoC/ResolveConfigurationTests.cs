using System;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class ResolveConfigurationTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ResolveConfigurationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CommandClass_DefaultUses_Resolve()
        {
            Assert.Throws<Exception>(() => new AppRunner<App>()
                    .UseDependencyResolver(new TestDependencyResolver())
                    .Run(new[] {"Do"}))
                .Message.Should().Contain(
                    "Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveConfigurationTests+App");
        }

        [Fact]
        public void CommandClass_CanConfigureToUse_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver(), useTryResolveForCommandClass: true)
                .RunInMem("Do", _testOutputHelper)
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void ArgumentModel_DefaultUses_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {new App()})
                .RunInMem("Do", _testOutputHelper)
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void ArgumentModel_CanConfigureToUse_Resolve()
        {
            Assert.Throws<Exception>(() => new AppRunner<App>()
                    .UseDependencyResolver(new TestDependencyResolver { new App() }, useResolveForArgumentModel: true)
                    .Run(new[] { "Do" }))
                .Message.Should().Contain(
                    "Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveConfigurationTests+ArgModel");
        }

        class App
        {
            private TestOutputs TestOutputs { get; set; }
            public void Do(ArgModel argModel)
            {
                TestOutputs.Capture(argModel);
            }
        }

        public class ArgModel : IArgumentModel
        {

        }
    }
}