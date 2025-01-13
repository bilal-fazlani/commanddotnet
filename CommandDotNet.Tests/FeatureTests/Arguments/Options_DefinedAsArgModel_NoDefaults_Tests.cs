using System;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments;

public class Options_DefinedAsArgModel_NoDefaults_Tests
{
    private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
    private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

    public Options_DefinedAsArgModel_NoDefaults_Tests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void SampleTypes_BasicHelp()
    {
        new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
        {
            When = {Args = "ArgsDefaults -h"},
            Then =
            {
                Output = @"Usage: testhost.dll ArgsDefaults [options]

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
  --ObjectListArg"
            }
        });
    }

    [Fact]
    public void SampleTypes_DetailedHelp()
    {
        new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
        {
            When = {Args = "ArgsDefaults -h"},
            Then =
            {
                Output = @"Usage: testhost.dll ArgsDefaults [options]

Options:

  --BoolArg

  --StringArg                 <TEXT>

  --StructArg                 <NUMBER>

  --StructNArg                <NUMBER>

  --EnumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectArg                 <URI>

  --StringListArg (Multiple)  <TEXT>

  --StructListArg (Multiple)  <NUMBER>

  --EnumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectListArg (Multiple)  <URI>"
            }
        });
    }

    [Fact]
    public void SampleTypes_Exec_Named()
    {
        new AppRunner<OptionsNoDefaults>().Verify(new Scenario
        {
            When = {Args = "ArgsDefaults --StringArg green --StructArg 1 --StructNArg 2 " +
                           "--EnumArg Monday --ObjectArg http://google.com " +
                           "--StringListArg yellow --StringListArg orange " +
                           "--StructListArg 23 --StructListArg 5 " +
                           "--EnumListArg Friday --EnumListArg Tuesday " +
                           "--ObjectListArg http://apple.com --ObjectListArg http://github.com"},
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(
                    new OptionsNoDefaultsSampleTypesModel
                    {
                        StringArg = "green",
                        StructArg = 1,
                        StructNArg = 2,
                        EnumArg = DayOfWeek.Monday,
                        ObjectArg = new Uri("http://google.com"),
                        StringListArg = ["yellow", "orange"],
                        StructListArg = [23, 5],
                        EnumListArg = [DayOfWeek.Friday, DayOfWeek.Tuesday],
                        ObjectListArg =
                        [
                            new("http://apple.com"),
                            new("http://github.com")
                        ]
                    })
            }
        });
    }

    private class OptionsNoDefaults
    {
        public void ArgsDefaults(OptionsNoDefaultsSampleTypesModel model)
        {
        }
    }
}