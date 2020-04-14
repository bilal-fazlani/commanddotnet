using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class NestedArgModelTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public NestedArgModelTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NestedModel_BasicHelp_IncludesNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then = { Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  Operand1
  Operand2

Options:
  --Option1
  --Option2
" }
            });
        }

        [Fact]
        public void NestedModel_DetailedHelp_IncludesNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then = { Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  Operand1  <TEXT>

  Operand2  <TEXT>

Options:

  --Option1  <TEXT>

  --Option2  <TEXT>
" }
            });
        }

        [Fact]
        public void NestedModel_Exec_MapsNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do --Option1 aaa --Option2 bbb ccc ddd",
                Then =
                {
                    Captured =
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

        private class NestedModelApp
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do(ParentModel parameterModel)
            {
                TestCaptures.Capture(parameterModel);
            }
        }

        private class ParentModel: IArgumentModel
        {
            [Option]
            public string Option1 { get; set; }

            [Operand]
            public string Operand1 { get; set; }

            public NestedModel NestedModel { get; set; }
        }

        private class NestedModel : IArgumentModel
        {
            [Option]
            public string Option2 { get; set; }

            [Operand]
            public string Operand2 { get; set; }
        }
    }
}