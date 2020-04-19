using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsArgModel_Defaults_Tests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsArgModel_Defaults_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp_IncludesAll()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

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
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp_IncludesAll()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --BoolArg                                [True]

  --StringArg                 <TEXT>       [lala]

  --StructArg                 <NUMBER>     [3]

  --StructNArg                <NUMBER>     [4]

  --EnumArg                   <DAYOFWEEK>  [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectArg                 <URI>        [http://google.com/]

  --StringListArg (Multiple)  <TEXT>       [red, blue]

  --StructListArg (Multiple)  <NUMBER>     [3, 4]

  --EnumListArg (Multiple)    <DAYOFWEEK>  [Monday, Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectListArg (Multiple)  <URI>        [http://google.com/, http://github.com/]
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsCanBeAssignedByName()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                When = {Args = "ArgsDefaults --StringArg green --StructArg 1 --StructNArg 2 " +
                           "--EnumArg Monday --ObjectArg http://google.com " +
                           "--StringListArg yellow --StringListArg orange " +
                           "--StructListArg 23 --StructListArg 5 " +
                           "--EnumListArg Friday --EnumListArg Tuesday " +
                           "--ObjectListArg http://apple.com --ObjectListArg http://github.com"},
                Then =
                {
                    Captured = { new OptionsDefaultsSampleTypesModel
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
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                When = {Args = "ArgsDefaults"},
                Then =
                {
                    Captured = { new OptionsDefaultsSampleTypesModel() }
                }
            });
        }

        private class OptionsDefaults
        {
            private TestCaptures TestCaptures { get; set; }

            public void ArgsDefaults(OptionsDefaultsSampleTypesModel model)
            {
                TestCaptures.Capture(model);
            }
        }
    }
}
