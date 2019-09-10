using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class SubCommandInterceptorOptionTests : TestBase
    {
        public SubCommandInterceptorOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GivenRoot_HelpIncludesInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --rootOpt           <TEXT>

  --inheritedRootOpt  <TEXT>

Commands:

  Leaf
  RootDo

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void GivenRoot_HelpForSubCommand_IncludesInheritedInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "RootDo -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll RootDo [arguments] [options]

Arguments:

  DoArg  <TEXT>

Options:

  --DoOpt             <TEXT>

  --inheritedRootOpt  <TEXT>"
                }
            });
        }

        [Fact]
        public void GivenLeaf_HelpIncludesOnlyLocalInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Leaf [command] [options]

Options:

  --LeafOpt           <TEXT>

  --InheritedLeafOpt  <TEXT>

Commands:

  LeafDo

Use ""dotnet testhost.dll Leaf [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void GivenLeaf_HelpForSubCommand_IncludesLocalInheritedInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf LeafDo -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Leaf LeafDo [arguments] [options]

Arguments:

  DoArg  <TEXT>

Options:

  --DoOpt             <TEXT>

  --InheritedLeafOpt  <TEXT>"
                }
            });
        }

        [Fact]
        public void GivenRoot_ExecutingLocalSubcommand_ParsesInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "--rootOpt rov --inheritedRootOpt irov RootDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new RootInterceptorResult{RootOpt = "rov", InheritedRootOpt = "irov"},
                        new RootDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenRoot_ExecutingLocalSubcommand_InheritedOptionCanBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "--rootOpt rov RootDo --DoOpt a b --inheritedRootOpt irov",
                Then =
                {
                    Outputs =
                    {
                        new RootInterceptorResult{RootOpt = "rov", InheritedRootOpt = "irov"},
                        new RootDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenRoot_ExecutingLocalSubcommand_NonInheritedOptionCanNotBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "--inheritedRootOpt irov RootDo --DoOpt a b --rootOpt rov",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized option '--rootOpt'" }
                }
            });
        }

        [Fact]
        public void GivenLeaf_ExecutingLocalSubcommand_ParsesInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt lov --InheritedLeafOpt ilov LeafDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new LeafInterceptorResult{LeafOpt = "lov", InheritedLeafOpt = "ilov"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenLeaf_ExecutingLocalSubcommand_InheritedOptionCanBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt lov LeafDo --DoOpt a b --InheritedLeafOpt ilov",
                Then =
                {
                    Outputs =
                    {
                        new LeafInterceptorResult{LeafOpt = "lov", InheritedLeafOpt = "ilov"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenLeaf_ExecutingLocalSubcommand_NonInheritedOptionCanNotBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --InheritedLeafOpt ilov LeafDo --DoOpt a b --LeafOpt lov",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized option '--LeafOpt'" }
                }
            });
        }

        [Fact]
        public void ExecutingSubCommandWillParseAndExecuteLocalInterceptorOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt leaf LeafDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new LeafInterceptorResult{LeafOpt = "leaf"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void InterceptorOptionMustBeSpecifiedBeforeLocalCommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf LeafDo --LeafOpt leaf --DoOpt a b",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized option '--LeafOpt'" }
                }
            });
        }

        public class Root
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Interceptor(
                CommandContext context, ExecutionDelegate next, 
                [Option] string rootOpt = null, [Option(Inherited = true)] string inheritedRootOpt = null)
            {
                TestOutputs.Capture(new RootInterceptorResult { RootOpt = rootOpt, InheritedRootOpt = inheritedRootOpt });
                return next(context);
            }

            public void RootDo(RootDoResult leafDoResult)
            {
                TestOutputs.Capture(leafDoResult);
            }

            [SubCommand]
            public Leaf Leaf { get; set; }
        }

        public class Leaf
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Interceptor(
                CommandContext context, ExecutionDelegate next, LeafInterceptorResult leafInterceptorResult)
            {
                TestOutputs.Capture(leafInterceptorResult);
                return next(context);
            }

            public void LeafDo(LeafDoResult leafDoResult)
            {
                TestOutputs.Capture(leafDoResult);
            }
        }

        public class RootInterceptorResult
        {
            public string RootOpt { get; set; }
            public string InheritedRootOpt { get; set; }
        }

        public class RootDoResult : IArgumentModel
        {
            [Option]
            public string DoOpt { get; set; }
            [Operand]
            public string DoArg { get; set; }
        }

        public class LeafInterceptorResult : IArgumentModel
        {
            [Option]
            public string LeafOpt { get; set; }

            [Option(Inherited = true)]
            public string InheritedLeafOpt { get; set; }
        }

        public class LeafDoResult : IArgumentModel
        {
            [Option]
            public string DoOpt { get; set; }
            [Operand]
            public string DoArg { get; set; }
        }
    }
}