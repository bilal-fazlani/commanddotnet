using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class HostCommandMiddlewareOptions : TestBase
    {
        public HostCommandMiddlewareOptions(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GivenRootMiddlewareHost_HelpIncludesMiddlewareOptions()
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
        public void GivenRootMiddlewareHost_HelpForHostedSubCommand_IncludesInheritedMiddlewareOptions()
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
        public void GivenLeafMiddlewareHost_HelpIncludesOnlyLocalMiddlewareOptions()
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
        public void GivenLeafMiddlewareHost_HelpForHostedSubCommand_IncludesLocalInheritedMiddlewareOptions()
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
        public void GivenRootMiddlewareHost_ExecutingLocalSubcommand_ParsesMiddlewareOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "--rootOpt rov --inheritedRootOpt irov RootDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new RootMiddlewareResult{RootOpt = "rov", InheritedRootOpt = "irov"},
                        new RootDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenRootMiddlewareHost_ExecutingLocalSubcommand_InheritedOptionCanBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "--rootOpt rov RootDo --DoOpt a b --inheritedRootOpt irov",
                Then =
                {
                    Outputs =
                    {
                        new RootMiddlewareResult{RootOpt = "rov", InheritedRootOpt = "irov"},
                        new RootDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenRootMiddlewareHost_ExecutingLocalSubcommand_NonInheritedOptionCanNotBeSpecifiedAfterSubcommand()
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
        public void GivenLeafMiddlewareHost_ExecutingLocalSubcommand_ParsesMiddlewareOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt lov --InheritedLeafOpt ilov LeafDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new LeafMiddlewareResult{LeafOpt = "lov", InheritedLeafOpt = "ilov"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenLeafMiddlewareHost_ExecutingLocalSubcommand_InheritedOptionCanBeSpecifiedAfterSubcommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt lov LeafDo --DoOpt a b --InheritedLeafOpt ilov",
                Then =
                {
                    Outputs =
                    {
                        new LeafMiddlewareResult{LeafOpt = "lov", InheritedLeafOpt = "ilov"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GivenLeafMiddlewareHost_ExecutingLocalSubcommand_NonInheritedOptionCanNotBeSpecifiedAfterSubcommand()
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
        public void ExecutingSubCommandWillParseAndExecuteLocalMiddlewareOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt leaf LeafDo --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new LeafMiddlewareResult{LeafOpt = "leaf"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void MiddlewareOptionMustBeSpecifiedBeforeLocalCommand()
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

            public Task<int> Middleware(
                CommandContext context, Func<CommandContext, Task<int>> next, 
                [Option] string rootOpt = null, [Option(Inherited = true)] string inheritedRootOpt = null)
            {
                TestOutputs.Capture(new RootMiddlewareResult { RootOpt = rootOpt, InheritedRootOpt = inheritedRootOpt });
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

            public Task<int> Middleware(
                CommandContext context, Func<CommandContext, Task<int>> next, LeafMiddlewareResult leafMiddlewareResult)
            {
                TestOutputs.Capture(leafMiddlewareResult);
                return next(context);
            }

            public void LeafDo(LeafDoResult leafDoResult)
            {
                TestOutputs.Capture(leafDoResult);
            }
        }

        public class RootMiddlewareResult
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

        public class LeafMiddlewareResult : IArgumentModel
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