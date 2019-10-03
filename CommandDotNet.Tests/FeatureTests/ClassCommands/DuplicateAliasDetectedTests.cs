using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DuplicateAliasDetectedTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DuplicateAliasDetectedTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDetectDuplicateCommands()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateCommandApp>()
                        .RunInMem("-h", _testOutputHelper))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'DuplicateCommandApp'. Duplicates: 'Command:lala' & 'Command:lala'");
        }

        [Fact]
        public void ShouldDetectDuplicateArgs()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateArgumentApp>()
                        .RunInMem("-h", _testOutputHelper))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'Do'. Duplicates: 'Option:lala' & 'Operand:lala'");
        }

        [Fact]
        public void ShouldDetectDuplicateInterceptorOptions()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateInterceptorOptionApp>()
                        .RunInMem("-h", _testOutputHelper))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'DuplicateInterceptorOptionApp'. Duplicates: 'Command:lala' & 'Option:lala'");
        }

        [Fact]
        public void ShouldDetectDuplicateInheritiedOptions()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateInheritedOptionApp>()
                        .RunInMem("SubApp SubDo -h", _testOutputHelper))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'SubDo'. Duplicates: 'Option:lala' & 'Operand:lala'");
        }

        class DuplicateCommandApp
        {
            [Command(Name = "lala")]
            public void Do1(string text) { }
            [Command(Name = "lala")]
            public void Do2(int number) { }
        }

        class DuplicateArgumentApp
        {
            public void Do(
                [Operand(Name = "lala")] string operand,
                [Option(LongName = "lala")] string option)
            { }
        }

        class DuplicateInterceptorOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, string lala)
            {
                return next();
            }

            [Command(Name = "lala")]
            public void Do() { }
        }

        class DuplicateInheritedOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, [Option(Inherited = true)] string lala)
            {
                return next();
            }

            public void Do() { }

            [SubCommand]
            public class SubApp
            {
                public void SubDo(string lala) { }
            }
        }
    }
}