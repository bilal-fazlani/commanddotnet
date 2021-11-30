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
        public ResolveStrategyTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void CommandClass_DefaultUses_Resolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver())
                .Expect<Exception>(ex =>
                    ex.Message.Should().Be("Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+App"))
                .RunInMem("Do");
        }

        [Fact]
        public void CommandClass_CanConfigureToUse_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver(), commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .RunInMem("Do")
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void CommandClass_CanConfigureToUse_ResolveOrThrow()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver { (App?)null },
                    commandClassResolveStrategy: ResolveStrategy.ResolveOrThrow)
                .RunInMem("Do")
                .Console.ErrorText().Should().Contain("The resolver returned null for type 'CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+App'");
        }

        [Fact]
        public void ArgumentModel_DefaultUses_TryResolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {new App()})
                .RunInMem("Do")
                .ExitCode.Should().Be(0);
        }

        [Fact]
        public void ArgumentModel_CanConfigureToUse_Resolve()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver { new App() },
                    argumentModelResolveStrategy: ResolveStrategy.Resolve)
                .Expect<Exception>(ex => 
                    ex.Message.Should().Be("Dependency not registered: CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+ArgModel"))
                .RunInMem("Do");
        }

        [Fact]
        public void ArgumentModel_CanConfigureToUse_ResolveOrThrow()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver { new App(), (ArgModel?)null }, argumentModelResolveStrategy: ResolveStrategy.ResolveOrThrow)
                .RunInMem("Do")
                .Console.ErrorText().Should().Contain("The resolver returned null for type 'CommandDotNet.Tests.CommandDotNet.IoC.ResolveStrategyTests+ArgModel'");
        }

        class App
        {
            public void Do(ArgModel argModel)
            {
            }
        }

        private class ArgModel : IArgumentModel
        {

        }
    }
}