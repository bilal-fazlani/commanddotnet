using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsArgModel_Defaults : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsArgModel_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp_IncludesAll()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:
  -h | --help      Show help information
  --BoolArg
  --StringArg
  --StructArg
  --StructNArg
  --EnumArg
  --ObjectArg
  --StringListArg
  --StructListArg
  --EnumListArg
  --ObjectListArg" }
            });
        }

        [Fact(Skip = "Help should not contain default values as [System.Collections.Generic.List`1[...]]")]
        public void SampleTypes_DetailedHelp_IncludesAll()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  -h | --help
  Show help information

  --BoolArg                     <BOOLEAN>      [True]
  Allowed values: true, false

  --StringArg                   <TEXT>         [lala]

  --StructArg                   <NUMBER>       [3]

  --StructNArg                  <NUMBER>       [4]

  --EnumArg                     <DAYOFWEEK>    [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectArg                   <URI>          [http://google.com/]

  --StringListArg (Multiple)    <TEXT>         [System.Collections.Generic.List`1[System.String]]

  --StructListArg (Multiple)    <NUMBER>       [System.Collections.Generic.List`1[System.Int32]]

  --EnumListArg (Multiple)      <DAYOFWEEK>    [System.Collections.Generic.List`1[System.DayOfWeek]]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectListArg (Multiple)    <URI>          [System.Collections.Generic.List`1[System.Uri]]" }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsCanBeAssignedByName()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "ArgsDefaults --StringArg green --StructArg 1 --StructNArg 2 " +
                           "--EnumArg Monday --ObjectArg http://google.com " +
                           "--StringListArg yellow --StringListArg orange " +
                           "--StructListArg 23 --StructListArg 5 " +
                           "--EnumListArg Friday --EnumListArg Tuesday " +
                           "--ObjectListArg http://apple.com --ObjectListArg http://github.com",
                Then =
                {
                    Outputs = { new OptionsDefaultsSampleTypesModel
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
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsAreNotRequired()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "ArgsDefaults",
                Then =
                {
                    Outputs = { new OptionsDefaultsSampleTypesModel() }
                }
            });
        }

        private class OptionsDefaults
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(OptionsDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}