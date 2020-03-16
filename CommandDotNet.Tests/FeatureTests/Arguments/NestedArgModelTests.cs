using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class NestedArgModelTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public NestedArgModelTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void NestedModel_BasicHelp_IncludesNestedOperandsAndOptions()
        {
            Verify(new Scenario<NestedModelApp>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  Operand1
  Operand2

Options:
  --Option1
  --Option2" }
            });
        }

        [Fact]
        public void NestedModel_DetailedHelp_IncludesNestedOperandsAndOptions()
        {
            Verify(new Scenario<NestedModelApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  Operand1  <TEXT>

  Operand2  <TEXT>

Options:

  --Option1  <TEXT>

  --Option2  <TEXT>" }
            });
        }

        [Fact]
        public void NestedModel_Exec_MapsNestedOperandsAndOptions()
        {
            Verify(new Scenario<NestedModelApp>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "Do --Option1 aaa --Option2 bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new ParentModel
                        {
                            Option1 = "aaa", Operand1 = "ccc",
                            NestedModel = new NestedModel {Option2 = "bbb", Operand2 = "ddd"}
                        }
                    }
                }
            });
        }

        public class NestedModelApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do(ParentModel parameterModel)
            {
                TestOutputs.Capture(parameterModel);
            }
        }

        public class ParentModel: IArgumentModel
        {
            [Option]
            public string Option1 { get; set; }

            [Operand]
            public string Operand1 { get; set; }

            public NestedModel NestedModel { get; set; }
        }

        public class NestedModel : IArgumentModel
        {
            [Option]
            public string Option2 { get; set; }

            [Operand]
            public string Operand2 { get; set; }
        }
    }
}