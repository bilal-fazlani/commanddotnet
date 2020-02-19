using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class ResolveArgumentModelsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ResolveArgumentModelsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldUseModelFromDependencyResolver()
        {
            var argModel = new ArgModel {Text = "some default"};
            var testOutputs = new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {new App(), argModel})
                .RunInMem("Do lala", _testOutputHelper)
                .TestOutputs;

            var resolvedArgModel = testOutputs.Get<ArgModel>();
            resolvedArgModel.Should().BeSameAs(argModel);
        }

        [Fact]
        public void HelpShouldDisplayDefaultFromResolvedModel()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver { new ArgModel { Text = "default from resolver" } })
                .RunInMem("Do -h", _testOutputHelper)
                .ConsoleOut.Should().Contain("default from resolver");
        }

        class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do(ArgModel argModel)
            {
                TestOutputs.Capture(argModel);
            }
        }

        class ArgModel : IArgumentModel
        {
            public string Text { get; set; }
        }
    }
}