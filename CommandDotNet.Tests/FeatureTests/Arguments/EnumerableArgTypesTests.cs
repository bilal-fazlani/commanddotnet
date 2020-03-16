using System.Collections.Generic;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class EnumerableArgTypesTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public EnumerableArgTypesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void EnumerableModel_BasicHelp_Includes_Arguments()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelp },
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
            Verify(new Scenario<App>
            {
                Given = { AppSettings = DetailedHelp },
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
            Verify(new Scenario<App>
            {
                Given = {AppSettings = BasicHelp},
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
            Verify(new Scenario<App>
            {
                Given = {AppSettings = DetailedHelp},
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
            Verify(new Scenario<App>
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
            Verify(new Scenario<App>
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
            Verify(new Scenario<App>
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
            Verify(new Scenario<App>
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

        public class App
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

        public class EnumerableModel : IArgumentModel
        {
            [Option]
            public IEnumerable<string> Options { get; set; }

            public IEnumerable<string> Args { get; set; }
        }
    }
}