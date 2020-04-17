using System.Threading;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class ConstructorInjectionTests
    {
        private readonly ITestOutputHelper _output;

        public ConstructorInjectionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldInstantiateCommandClassUsingCtorWithMostInjectableServices()
        {
            var result = new AppRunner<UseLargestCtorApp>().RunInMem("Do", _output);

            result.TestCaptures.Get<CommandContext>().Should().NotBeNull();
            result.TestCaptures.Get<TestConsole>().Should().NotBeNull();
            result.TestCaptures.Get<CancellationToken>().Should().NotBeNull();
        }

        class UseLargestCtorApp
        {
            public TestCaptures TestCaptures { get; set; }

            private readonly CommandContext _commandContext;
            private readonly IConsole _console;
            private readonly CancellationToken _cancellationToken;

            public UseLargestCtorApp()
            {
            }

            public UseLargestCtorApp(CommandContext commandContext)
            {
                _commandContext = commandContext;
            }

            public UseLargestCtorApp(CommandContext commandContext, IConsole console)
            {
                _commandContext = commandContext;
                _console = console;
            }

            public UseLargestCtorApp(CommandContext commandContext, IConsole console, CancellationToken cancellationToken)
            {
                _commandContext = commandContext;
                _console = console;
                _cancellationToken = cancellationToken;
            }

            public void Do()
            {
                TestCaptures.Capture(_cancellationToken);
                TestCaptures.Capture(_console);
                TestCaptures.Capture(_commandContext);
            }
        }
    }
}