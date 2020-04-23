using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsArgModel_Defaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsArgModel_Defaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp_IncludesAll()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg
" }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp_IncludesAll()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

Arguments:

  BoolArg                   <BOOLEAN>    [True]
  Allowed values: true, false

  StringArg                 <TEXT>       [lala]

  StructArg                 <NUMBER>     [3]

  StructNArg                <NUMBER>     [4]

  EnumArg                   <DAYOFWEEK>  [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                 <URI>        [http://google.com/]

  StringListArg (Multiple)  <TEXT>       [red, blue]
" }
            });
        }

        [Fact]
        public void StructList_BasicHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:
  StructListArg
" }
            });
        }

        [Fact]
        public void StructList_DetailedHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:

  StructListArg (Multiple)  <NUMBER>  [3, 4]
" }
            });
        }

        [Fact]
        public void EnumList_BasicHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

Arguments:
  EnumListArg
" }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

Arguments:

  EnumListArg (Multiple)  <DAYOFWEEK>  [Monday, Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
" }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:
  ObjectListArg
" }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp_IncludesList()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then = { Output = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:

  ObjectListArg (Multiple)  <URI>  [http://google.com/, http://github.com/]
" }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsAreAssignedByPosition()
        {
            new AppRunner<OperandsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsDefaultsSampleTypesModel
                        {
                            BoolArg = true,
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string> {"yellow", "orange"}
                        })
                }
            });
        }

        [Fact]
        public void SampleType_Exec_OperandsAreNotRequired_UsesDefaults()
        {
            new AppRunner<OperandsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsDefaultsSampleTypesModel
                        {
                            StringArg = "lala",
                            StructArg = 3,
                            StructNArg = 4,
                            EnumArg = DayOfWeek.Tuesday,
                        })
                }
            });
        }

        private class OperandsDefaults
        {
            public void ArgsDefaults(OperandsDefaultsSampleTypesModel model)
            {
            }

            public void StructListDefaults(OperandsDefaultsStructListArgumentModel model)
            {
            }

            public void EnumListDefaults(OperandsDefaultsEnumListArgumentModel model)
            {
            }

            public void ObjectListDefaults(OperandsDefaultsObjectListArgumentModel model)
            {
            }
        }
    }
}
