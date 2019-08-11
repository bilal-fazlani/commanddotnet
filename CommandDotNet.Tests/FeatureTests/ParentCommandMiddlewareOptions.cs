using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParentCommandMiddlewareOptions : TestBase
    {
        public ParentCommandMiddlewareOptions(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void HelpIncludesGlobalOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --rootOpt  <TEXT>

Commands:

  Leaf

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void HelpForSubCommandIncludesOnlyLocalGlobalOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Leaf [command] [options]

Options:

  --LeafOpt  <TEXT>

Commands:

  Do

Use ""dotnet testhost.dll Leaf [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void ExecutingSubCommandWillParseAndExecuteLocalGlobalOptions()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf --LeafOpt leaf Do --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        new LeafGlobalResult{LeafOpt = "leaf"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void GlobalOptionMustBeSpecifiedBeforeLocalCommand()
        {
            Verify(new Scenario<Root>
            {
                WhenArgs = "Leaf Do --LeafOpt leaf --DoOpt a b",
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

            public Task<int> Middleware(CommandContext context, Func<CommandContext, Task<int>> next, [Option] string rootOpt = null)
            {
                TestOutputs.Capture(new RootGlobalResult { RootOpt = rootOpt });
                return next(context);
            }

            [SubCommand]
            public Leaf Leaf { get; set; }
        }

        public class Leaf
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public Task<int> Middleware(CommandContext context, Func<CommandContext, Task<int>> next, LeafGlobalResult leafGlobalResult)
            {
                TestOutputs.Capture(leafGlobalResult);
                return next(context);
            }

            public void Do(LeafDoResult leafDoResult)
            {
                TestOutputs.Capture(leafDoResult);
            }
        }

        public class RootGlobalResult
        {
            [Option]
            public string RootOpt { get; set; }
        }

        public class LeafGlobalResult : IArgumentModel
        {
            [Option]
            public string LeafOpt { get; set; }
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