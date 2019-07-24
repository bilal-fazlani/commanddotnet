using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ConstructorOptionsInherited : TestBase
    {
        public ConstructorOptionsInherited(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void HelpIncludesGlobalOptions()
        {
            Verify(new Given<Root>
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
        public void HelpForSubCommandIncludesLocalAndInheritedRootGlobalOptions()
        {
            Verify(new Given<Root>
            {
                WhenArgs = "Leaf -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Leaf [command] [options]

Options:

  --LeafOpt  <TEXT>

  --rootOpt  <TEXT>

Commands:

  Do

Use ""dotnet testhost.dll Leaf [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void ExecutingSubCommandWillParseAndExecuteLocalGlobalOptions()
        {
            Verify(new Given<Root>
            {
                WhenArgs = "--rootOpt root Leaf --LeafOpt leaf Do --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        //new RootGlobalResult{RootOpt = "root"},
                        new LeafGlobalResult{LeafOpt = "leaf"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        [Fact]
        public void InheritedGlobalOptionCanBeSpecifiedAfterLocalCommand()
        {
            Verify(new Given<Root>
            {
                WhenArgs = "Leaf Do --LeafOpt leaf --rootOpt root --DoOpt a b",
                Then =
                {
                    Outputs =
                    {
                        //new RootGlobalResult{RootOpt = "root"},
                        new LeafGlobalResult{LeafOpt = "leaf"},
                        new LeafDoResult{DoOpt = "a", DoArg = "b"}
                    }
                }
            });
        }

        public class Root
        {
            private readonly string _rootOpt;

            private TestOutputs _testOutputs;

            [InjectProperty]
            public TestOutputs TestOutputs
            {
                get => _testOutputs;
                set
                {
                    _testOutputs = value;
                    _testOutputs.Capture(new RootGlobalResult{RootOpt = _rootOpt});
                }
            }

            public Root([Option(Inherited = true)] string rootOpt)
            {
                _rootOpt = rootOpt;
            }

            [SubCommand]
            public Leaf Leaf { get; set; }
        }

        public class Leaf
        {
            private readonly LeafGlobalResult _leafGlobalResult;

            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public Leaf(LeafGlobalResult leafGlobalResult)
            {
                // TestOutputs property isn't injected yet
                _leafGlobalResult = leafGlobalResult;
            }

            public void Do(LeafDoResult leafDoResult)
            {
                TestOutputs.Capture(_leafGlobalResult);
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
            [Option(Inherited = true)]
            public string LeafOpt { get; set; }
        }

        public class LeafDoResult : IArgumentModel
        {
            [Option]
            public string DoOpt { get; set; }
            [Operand]
            public string DoArg { get; set; }

            internal string InheritedRootOpt { get; set; }
        }
    }
}