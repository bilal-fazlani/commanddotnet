using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class EnumerableArgTypes : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public EnumerableArgTypes(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void EnumerableModel_BasicHelp_Includes_Arguments()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "EnumerableModel -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumerableModel [arguments] [options]

Arguments:
  Args

Options:
  --Options
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void EnumerableModel_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "EnumerableModel -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumerableModel [arguments] [options]

Arguments:

  Args (Multiple)    <TEXT>


Options:

  --Options (Multiple)    <TEXT>

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void EnumerableParams_BasicHelp_Includes_Arguments()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "Enumerable -h",
                Then = {Result = @"Usage: dotnet testhost.dll Enumerable [arguments] [options]

Arguments:
  args

Options:
  --options
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void EnumerableParams_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = DetailedHelp},
                WhenArgs = "Enumerable -h",
                Then = {Result = @"Usage: dotnet testhost.dll Enumerable [arguments] [options]

Arguments:

  args (Multiple)    <TEXT>


Options:

  --options (Multiple)    <TEXT>

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void EnumerableParams_Exec_MapsArguments()
        {
            Verify(new Given<App>
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
            Verify(new Given<App>
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
            Verify(new Given<App>
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

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

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
        }

        public class EnumerableModel : IArgumentModel
        {
            [Option]
            public IEnumerable<string> Options { get; set; }

            public IEnumerable<string> Args { get; set; }
        }
    }
}