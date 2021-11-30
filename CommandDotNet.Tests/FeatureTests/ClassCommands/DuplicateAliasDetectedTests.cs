using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DuplicateAliasDetectedTests
    {
        public DuplicateAliasDetectedTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldDetectDuplicateCommands()
        {
            new AppRunner<DuplicateCommandApp>()
                .RunInMem("-h")
                .Console.ErrorText().Should()
                .Be("CommandDotNet.InvalidConfigurationException: Duplicate alias 'lala' added to command 'DuplicateCommandApp'. Duplicates: " +
                    "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateCommandApp.Do2)' & " +
                    "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateCommandApp.Do1)'");
        }

        [Fact]
        public void ShouldDetectDuplicateArgs()
        {
            new AppRunner<DuplicateArgumentApp>()
                .RunInMem("-h")
                .Console.ErrorText().Should()
                .Be("CommandDotNet.InvalidConfigurationException: Duplicate alias 'lala' added to command 'Do'. Duplicates: " +
                    "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateArgumentApp.Do.option)' & " +
                    "'Operand:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateArgumentApp.Do.operand)'");
        }

        [Fact]
        public void ShouldDetectDuplicateInterceptorOptions()
        {
            new AppRunner<DuplicateInterceptorOptionApp>()
                .RunInMem("-h")
                .Console.ErrorText().Should()
                .Be("CommandDotNet.InvalidConfigurationException: Duplicate alias 'lala' added to command 'DuplicateInterceptorOptionApp'. Duplicates: " +
                    "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInterceptorOptionApp.Do)' & " +
                    "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInterceptorOptionApp.Intercept.lala)'");
        }

        [Fact]
        public void ShouldDetectDuplicateInheritedOptions()
        {
            new AppRunner<DuplicateInheritedOptionApp>()
                .RunInMem("SubApp SubDo -h")
                .Console.ErrorText().Should()
                .Be("CommandDotNet.InvalidConfigurationException: Duplicate alias 'lala' added to command 'SubDo'. Duplicates: " +
                    "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInheritedOptionApp.Intercept.lala)' & " +
                    "'Operand:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInheritedOptionApp+SubApp.SubDo.lala)'");
        }

        [Fact]
        public void InteceptorOptionsShouldNotCollideWithChildCommandOptions()
        {
            new AppRunner<NonDuplicateInheritedOptionApp>()
                .RunInMem("SubApp SubDo -h")
                .ExitCode.Should().Be(0);
        }

        class DuplicateCommandApp
        {
            [Command("lala")]
            public void Do1(string text) { }
            [Command("lala")]
            public void Do2(int number) { }
        }

        class DuplicateArgumentApp
        {
            public void Do(
                [Operand("lala")] string operand,
                [Option("lala")] string option)
            { }
        }

        class DuplicateInterceptorOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, string lala)
            {
                return next();
            }

            [Command("lala")]
            public void Do() { }
        }

        class DuplicateInheritedOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, [Option(AssignToExecutableSubcommands = true)] string lala)
            {
                return next();
            }

            public void Do() { }

            [Subcommand]
            public class SubApp
            {
                public void SubDo(string lala) { }
            }
        }

        class NonDuplicateInheritedOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, [Option] string lala)
            {
                return next();
            }

            public void Do() { }

            [Subcommand]
            public class SubApp
            {
                public void SubDo(string lala) { }
            }
        }
    }
}