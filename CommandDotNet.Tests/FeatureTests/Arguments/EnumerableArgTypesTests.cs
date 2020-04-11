using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class EnumerableArgTypesTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public EnumerableArgTypesTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EnumerableModel_BasicHelp_Includes_Arguments()
        {
            new AppRunner<App>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "EnumerableModel -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumerableModel [options] [arguments]

Arguments:
  Args

Options:
  --Options" }
            });
        }

        [Fact]
        public void EnumerableModel_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            new AppRunner<App>(DetailedHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "EnumerableModel -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumerableModel [options] [arguments]

Arguments:

  Args (Multiple)  <TEXT>

Options:

  --Options (Multiple)  <TEXT>" }
            });
        }

        [Fact]
        public void EnumerableParams_BasicHelp_Includes_Arguments()
        {
            new AppRunner<App>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Enumerable -h",
                Then = {Result = @"Usage: dotnet testhost.dll Enumerable [options] [arguments]

Arguments:
  args

Options:
  --options" }
            });
        }

        [Fact]
        public void EnumerableParams_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            new AppRunner<App>(DetailedHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Enumerable -h",
                Then = {Result = @"Usage: dotnet testhost.dll Enumerable [options] [arguments]

Arguments:

  args (Multiple)  <TEXT>

Options:

  --options (Multiple)  <TEXT>" }
            });
        }

        [Fact]
        public void EnumerableParams_Exec_MapsArguments()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Enumerable --options aaa --options bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new EnumerableModel
                        {
                            Options = new[] {"aaa", "bbb"},
                            Args = new[] {"ccc", "ddd"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void EnumerableModel_Exec_MapsArguments()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "EnumerableModel --Options aaa --Options bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new EnumerableModel
                        {
                            Options = new[] {"aaa", "bbb"},
                            Args = new[] {"ccc", "ddd"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void CollectionParams_Exec_MapsArguments()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Collection --options aaa --options bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new EnumerableModel
                        {
                            Options = new[] {"aaa", "bbb"},
                            Args = new[] {"ccc", "ddd"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void ArrayParams_Exec_MapsArguments()
        {
            new AppRunner<App>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Array --options aaa --options bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new EnumerableModel
                        {
                            Options = new[] {"aaa", "bbb"},
                            Args = new[] {"ccc", "ddd"}
                        }
                    }
                }
            });
        }

        private class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void List([Option]List<string> options, List<string> args)
            {
                TestOutputs.Capture(new EnumerableModel { Options = options, Args = args });
            }

            public void Enumerable([Option]IEnumerable<string> options, IEnumerable<string> args)
            {
                TestOutputs.Capture(new EnumerableModel{Options = options, Args = args});
            }

            public void EnumerableModel(EnumerableModel model)
            {
                TestOutputs.Capture(model);
            }

            public void Collection([Option]ICollection<string> options, ICollection<string> args)
            {
                TestOutputs.Capture(new EnumerableModel { Options = options, Args = args });
            }

            public void Array([Option]string[] options, string[] args) => TestOutputs.Capture(new EnumerableModel { Options = options, Args = args });
        }

        private class EnumerableModel : IArgumentModel
        {
            [Option]
            public IEnumerable<string> Options { get; set; }

            public IEnumerable<string> Args { get; set; }
        }
    }
}