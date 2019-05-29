using System;
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

  stringArg    <TEXT>


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

  stringArg    <TEXT>    [some-default]


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("executes with value passed to nulled string")
                {
                    WhenArgs = "NullString some-value",
                    Then = {Outputs = {new RunResults {StringArg = "some-value"}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to nulled string")
                {
                    WhenArgs = "NullString",
                    Then = {Outputs = {new RunResults{StringArg = null}}}
                },
                new Given<OptionalParamsApp>("executes with value passed to defaulted string")
                {
                    WhenArgs = "DefaultString some-value",
                    Then = {Outputs = {new RunResults {StringArg = "some-value"}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to defaulted string")
                {
                    WhenArgs = "DefaultString",
                    Then = {Outputs = {new RunResults{StringArg = "some-default"}}}
                },
                new Given<OptionalParamsApp>("help displays args defined with nulled object")
                {
                    WhenArgs = "NullObject -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll NullObject [arguments] [options]

Arguments:

  uriArg    <URI>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("help displays args defined with nulled object")
                {
                    WhenArgs = "NullObject -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll NullObject [arguments] [options]

Arguments:

  uriArg    <URI>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("executes with value passed to nulled object")
                {
                    WhenArgs = "NullObject http://google.com",
                    Then = {Outputs = {new RunResults {UriArg = new Uri("http://google.com")}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to nulled object")
                {
                    WhenArgs = "NullObject",
                    Then = {Outputs = {new RunResults{UriArg = null}}}
                },
                new Given<OptionalParamsApp>("help displays args defined with defaulted int")
                {
                    WhenArgs = "DefaultInt -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll DefaultInt [arguments] [options]

Arguments:

  intArg    <NUMBER>    [1]


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("executes with value passed to defaulted int")
                {
                    WhenArgs = "DefaultInt 5",
                    Then = {Outputs = {new RunResults {IntArg = 5}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to defaulted int")
                {
                    WhenArgs = "DefaultInt",
                    Then = {Outputs = {new RunResults {IntArg = 1}}}
                },
                new Given<OptionalParamsApp>("help displays args defined with nulled int?")
                {
                    WhenArgs = "NullNullableInt -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll NullNullableInt [arguments] [options]

Arguments:

  intArg    <NUMBER>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("help displays args defined with defaulted int?")
                {
                    WhenArgs = "DefaultNullableInt -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll DefaultNullableInt [arguments] [options]

Arguments:

  intArg    <NUMBER>    [1]


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OptionalParamsApp>("executes with value passed to nulled int?")
                {
                    WhenArgs = "NullNullableInt 5",
                    Then = {Outputs = {new RunResults {IntArg = 5}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to nulled int?")
                {
                    WhenArgs = "NullNullableInt",
                    Then = {Outputs = {new RunResults {IntArg = null}}}
                },
                new Given<OptionalParamsApp>("executes with value passed to defaulted int?")
                {
                    WhenArgs = "DefaultNullableInt 5",
                    Then = {Outputs = {new RunResults {IntArg = 5}}}
                },
                new Given<OptionalParamsApp>("executes with no value passed to defaulted int?")
                {
                    WhenArgs = "DefaultNullableInt",
                    Then = {Outputs = {new RunResults {IntArg = 1}}}
                },
            };

        private class OptionalParamsApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void NullString(string stringArg = null)
            {
                TestOutputs.Capture(new RunResults{StringArg = stringArg});
            }

            public void DefaultString(string stringArg = "some-default")
            {
                TestOutputs.Capture(new RunResults { StringArg = stringArg });
            }

            public void NullObject(Uri uriArg = null)
            {
                TestOutputs.Capture(new RunResults { UriArg = uriArg });
            }

            public void NullNullableInt(int? intArg = null)
            {
                TestOutputs.Capture(new RunResults { IntArg = intArg });
            }

            public void DefaultNullableInt(int? intArg = 1)
            {
                TestOutputs.Capture(new RunResults { IntArg = intArg });
            }

            public void DefaultInt(int intArg = 1)
            {
                TestOutputs.Capture(new RunResults { IntArg = intArg });
            }
        }

        private class RunResults
        {
            public string StringArg { get; set; }
            public Uri UriArg { get; set; }
            public int? IntArg { get; set; }
        }
    }
}