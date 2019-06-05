using CommandDotNet.Attributes;
using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class NestedCommandScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<ThreeLevelsApp>("help includes 1st level commands and 2nd level app")
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


Commands:

  Do1
  Second

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<ThreeLevelsApp>("help for 2nd level app includes 2nd level commands and 3rd level app")
                {
                    WhenArgs = "Second -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Second [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  Do2
  Third

Use ""dotnet testhost.dll Second [command] --help"" for more information about a command."
                    }
                },
                new Given<ThreeLevelsApp>("help for 3rd level app includes 3rd level commands")
                {
                    WhenArgs = "Second Third -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Second Third [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  Do3

Use ""dotnet testhost.dll Second Third [command] --help"" for more information about a command."
                    }
                },
                new Given<ThreeLevelsApp>("can execute 1st level local command")
                {
                    WhenArgs = "Do1 --Opt1 1111 somearg",
                    Then = {Outputs = {new ArgModel1 {Opt1 = "1111", Arg1 = "somearg"}}}
                },
                new Given<ThreeLevelsApp>("can execute 2nd level local command")
                {
                    WhenArgs = "Second Do2 --Opt2 1111 somearg",
                    Then = {Outputs = {new ArgModel2 {Opt2 = "1111", Arg2 = "somearg"}}}
                },
                new Given<ThreeLevelsApp>("can execute 3rd level local command")
                {
                    WhenArgs = "Second Third Do3 --Opt3 1111 somearg",
                    Then = {Outputs = {new ArgModel3 {Opt3 = "1111", Arg3 = "somearg"}}}
                }
            };

        public class ThreeLevelsApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [SubCommand]
            public Second Second { get; set; }

            public void Do1(ArgModel1 model)
            {
                TestOutputs.Capture(model);
            }
        }

        public class Second
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [SubCommand]
            public Third Third { get; set; }

            public void Do2(ArgModel2 model)
            {
                TestOutputs.Capture(model);
            }
        }

        public class Third
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do3(ArgModel3 model)
            {
                TestOutputs.Capture(model);
            }
        }

        public class ArgModel1 : IArgumentModel
        {
            [Option]
            public string Opt1 { get; set; }
            [Argument]
            public string Arg1 { get; set; }
        }

        public class ArgModel2 : IArgumentModel
        {
            [Option]
            public string Opt2 { get; set; }
            [Argument]
            public string Arg2 { get; set; }
        }

        public class ArgModel3 : IArgumentModel
        {
            [Option]
            public string Opt3 { get; set; }
            [Argument]
            public string Arg3 { get; set; }
        }
    }
}