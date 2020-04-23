using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsArgModel_NoDefaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsArgModel_NoDefaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:

  BoolArg                   <BOOLEAN>
  Allowed values: true, false

  StringArg                 <TEXT>

  StructArg                 <NUMBER>

  StructNArg                <NUMBER>

  EnumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                 <URI>

  StringListArg (Multiple)  <TEXT>
"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:
  StructListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:

  StructListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:
  EnumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:

  EnumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:
  ObjectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:

  ObjectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault true green 1 2 Monday http://google.com yellow orange"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsNoDefaultsSampleTypesModel
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
        public void SampleTypes_Exec_OperandsNotRequired()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsNoDefaultsSampleTypesModel
                        {
                            StructArg = default(int),
                            EnumArg = default(DayOfWeek),
                        })
                }
            });
        }

        [Fact]
        public void StructList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "StructListNoDefault 23 5 7"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsNoDefaultsStructListArgumentModel
                        {
                            StructListArg = new List<int>{23,5,7}
                        })
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault Friday Tuesday Thursday"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsNoDefaultsEnumListArgumentModel
                        {
                            EnumListArg = new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday}
                        })
                }
            });
        }

        [Fact]
        public void ObjectList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault http://google.com http://apple.com http://github.com"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OperandsNoDefaultsObjectListArgumentModel
                        {
                            ObjectListArg = new List<Uri>
                            {
                                new Uri("http://google.com"),
                                new Uri("http://apple.com"),
                                new Uri("http://github.com"),
                            }
                        })
                }
            });
        }

        private class OperandsNoDefaults
        {
            public void ArgsNoDefault(OperandsNoDefaultsSampleTypesModel model)
            {
            }

            public void StructListNoDefault(OperandsNoDefaultsStructListArgumentModel model)
            {
            }

            public void EnumListNoDefault(OperandsNoDefaultsEnumListArgumentModel model)
            {
            }

            public void ObjectListNoDefault(OperandsNoDefaultsObjectListArgumentModel model)
            {
            }
        }
    }
}