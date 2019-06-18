using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsArgModel_NoDefaults : ScenarioTestBase<Options_DefinedAsArgModel_NoDefaults>
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsArgModel_NoDefaults(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OptionsNoDefaults>("SampleTypes - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:
  --BoolArg
  --StringArg
  --StructArg
  --StructNArg
  --EnumArg
  --ObjectArg
  --StringListArg
  --StructListArg
  --EnumListArg
  --ObjectListArg
  -h | --help      Show help information" }
                },
                new Given<OptionsNoDefaults>("SampleTypes - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --BoolArg

  --StringArg                   <TEXT>

  --StructArg                   <NUMBER>

  --StructNArg                  <NUMBER>

  --EnumArg                     <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectArg                   <URI>

  --StringListArg (Multiple)    <TEXT>

  --StructListArg (Multiple)    <NUMBER>

  --EnumListArg (Multiple)      <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectListArg (Multiple)    <URI>

  -h | --help
  Show help information" }
                },
                new Given<OptionsNoDefaults>("SampleTypes - Exec - named")
                {
                    WhenArgs = "ArgsDefaults --StringArg green --StructArg 1 --StructNArg 2 " +
                               "--EnumArg Monday --ObjectArg http://google.com " +
                               "--StringListArg yellow --StringListArg orange " +
                               "--StructListArg 23 --StructListArg 5 " +
                               "--EnumListArg Friday --EnumListArg Tuesday " +
                               "--ObjectListArg http://apple.com --ObjectListArg http://github.com",
                    Then =
                    {
                        Outputs = { new OptionsNoDefaultsSampleTypesModel
                        {
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string>{"yellow", "orange"},
                            StructListArg = new List<int>{23,5},
                            EnumListArg = new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday},
                            ObjectListArg = new List<Uri>
                            {
                                new Uri("http://apple.com"),
                                new Uri("http://github.com"),
                            }
                        } }
                    }
                },
                new Given<OptionsNoDefaults>("SampleTypes - Exec - options not required")
                {
                    WhenArgs = "ArgsDefaults",
                    Then =
                    {
                        Outputs = { new OptionsNoDefaultsSampleTypesModel() }
                    }
                },
            };

        private class OptionsNoDefaults
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(OptionsNoDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}