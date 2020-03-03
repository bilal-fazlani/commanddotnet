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
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstructorInjectionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldInstantiateCommandClassUsingCtorWithMostInjectableServices()
        {
            var result = new AppRunner<UseLargestCtorApp>().RunInMem("Do", _testOutputHelper);

            result.TestOutputs.Get<CommandContext>().Should().NotBeNull();
            result.TestOutputs.Get<TestConsole>().Should().NotBeNull();
            result.TestOutputs.Get<CancellationToken>().Should().NotBeNull();
        }

        class UseLargestCtorApp
        {
            public TestOutputs TestOutputs { get; set; }

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
                TestOutputs.Capture(_cancellationToken);
                TestOutputs.Capture(_console);
                TestOutputs.Capture(_commandContext);
            }
        }
    }
}