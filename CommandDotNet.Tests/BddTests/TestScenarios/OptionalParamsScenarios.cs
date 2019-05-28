using CommandDotNet.Attributes;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class OptionalParamsScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OptionalParamsApp>("help displays args defined with nulled string")
                {
                    WhenArgs = "NullString -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll NullString [arguments] [options]

Arguments:

  arg1    <TEXT>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("help displays args defined with defaulted string")
                {
                    WhenArgs = "DefaultString -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll DefaultString [arguments] [options]

Arguments:

  arg1    <TEXT>    [some-default]


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("executes with value passed to nulled string")
                {
                    WhenArgs = "NullString some-value",
                    Then = {Outputs = {new RunResults {Arg1 = "some-value"}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to nulled string")
                {
                    WhenArgs = "NullString",
                    Then = {Outputs = {new RunResults()}}
                },
                new Given<OptionalParamsApp>("executes with value passed to defaulted string")
                {
                    WhenArgs = "DefaultString some-value",
                    Then = {Outputs = {new RunResults {Arg1 = "some-value"}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to defaulted string")
                {
                    WhenArgs = "DefaultString",
                    Then = {Outputs = {new RunResults{Arg1 = "some-default"}}}
                }
            };

        private class OptionalParamsApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void NullString(string arg1 = null)
            {
                TestOutputs.Capture(new RunResults{Arg1 = arg1});
            }

            public void DefaultString(string arg1 = "some-default")
            {
                TestOutputs.Capture(new RunResults { Arg1 = arg1 });
            }
        }

        private class RunResults
        {
            public string Arg1 { get; set; }
        }
    }
}