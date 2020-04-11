using System;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class ResolveStrategyTests
    {
        private readonly ITestOutputHelper _output;

        public ResolveStrategyTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CommandClass_DefaultUses_Resolve()
        {
            Assert.Throws<Exception>(() => new AppRunner<App>()
                    .UseDependencyResolver(new TestDependencyResolver())
                    .Run(new[] {"Do"}))
                .Message.Should().Contain(
                    "Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+App");
        }

        [Fact]
        public void CommandClass_CanConfigureToUse_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver(), commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .RunInMem("Do", _output)
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void CommandClass_CanConfigureToUse_ResolveOrThrow()
        {
            Assert.Throws<ResolverReturnedNullException>(() => new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {(App) null},
                    commandClassResolveStrategy: ResolveStrategy.ResolveOrThrow)
                .RunInMem("Do", _output)
            ).Message.Should().Contain("The resolver returned null for type 'CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+App'");
        }

        [Fact]
        public void ArgumentModel_DefaultUses_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {new App()})
                .RunInMem("Do", _output)
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void ArgumentModel_CanConfigureToUse_Resolve()
        {
            Assert.Throws<Exception>(() => new AppRunner<App>()
                    .UseDependencyResolver(new TestDependencyResolver { new App() }, argumentModelResolveStrategy: ResolveStrategy.Resolve)
                    .Run(new[] { "Do" }))
                .Message.Should().Contain(
                    "Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+ArgModel");
        }

        [Fact]
        public void ArgumentModel_CanConfigureToUse_ResolveOrThrow()
        {
            Assert.Throws<ResolverReturnedNullException>(() => new AppRunner<App>()
                    .UseDependencyResolver(new TestDependencyResolver { new App(), (ArgModel)null }, argumentModelResolveStrategy: ResolveStrategy.ResolveOrThrow)
                    .Run(new[] { "Do" })
            ).Message.Should().Contain("The resolver returned null for type 'CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+ArgModel'");
        }

        class App
        {
            private TestCaptures TestCaptures { get; set; }
            public void Do(ArgModel argModel)
            {
                TestCaptures.Capture(argModel);
            }
        }

        public class ArgModel : IArgumentModel
        {

        }
    }
}