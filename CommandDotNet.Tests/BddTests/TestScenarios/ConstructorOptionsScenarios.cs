using CommandDotNet.Attributes;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class ConstructorOptionsScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<Root>("help includes global options")
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options] [command]

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
                new Given<Root>("help for sub-command includes only local global options")
                {
                    WhenArgs = "Leaf -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll Leaf [options] [command]

Options:

  -h | --help
  Show help information

  --LeafOpt      <TEXT>


Commands:

  Do

Use ""dotnet testhost.dll Leaf [command] --help"" for more information about a command."
                    }
                },
                new Given<Root>("executing sub-command will parse and execute local global options")
                {
                    WhenArgs = "Leaf --LeafOpt leaf Do --DoOpt a b",
                    Then =
                    {
                        OutputsStrict = true,
                        Outputs =
                        {
                            new LeafGlobalResult{LeafOpt = "leaf"},
                            new LeafDoResult{DoOpt = "a", DoArg = "b"}
                        }
                    }
                },
                new Given<Root>("global option must be specified before local command")
                {
                    WhenArgs = "Leaf Do --LeafOpt leaf --DoOpt a b",
                    Then =
                    {
                        ExitCode = 1,
                        HelpContainsTexts = { "Unrecognized option '--LeafOpt'" }
                    }
                }
            };

        public class Root
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public Root([Option] string rootOpt)
            {
                TestOutputs.Capture(new RootGlobalResult{RootOpt = rootOpt});
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
            [Option]
            public string LeafOpt { get; set; }
        }

        public class LeafDoResult : IArgumentModel
        {
            [Option]
            public string DoOpt { get; set; }
            [Argument]
            public string DoArg { get; set; }
        }
    }
}