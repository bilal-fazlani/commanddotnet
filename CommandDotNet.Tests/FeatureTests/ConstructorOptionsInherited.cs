using CommandDotNet.Attributes;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ConstructorOptionsInherited : ScenarioTestBase<ConstructorOptionsInherited>
    {
        public ConstructorOptionsInherited(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<Root>("help includes global options")
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information

  --rootOpt         <TEXT>


Commands:

  Leaf

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<Root>("help for sub-command includes local and inherited root global options")
                {
                    WhenArgs = "Leaf -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Leaf [options] [command]

Options:

  -h | --help
  Show help information

  --LeafOpt      <TEXT>

  --rootOpt      <TEXT>


Commands:

  Do

Use ""dotnet testhost.dll Leaf [command] --help"" for more information about a command."
                    }
                },
                new Given<Root>("executing sub-command will parse and execute local global options")
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
                },
                new Given<Root>("global option can be specified after local command")
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
                }
            };

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
            [Argument]
            public string DoArg { get; set; }

            internal string InheritedRootOpt { get; set; }
        }
    }
}