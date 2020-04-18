using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsMethodParams_NoDefaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_NoDefaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:
  boolArg
  stringArg
  structArg
  structNArg
  enumArg
  objectArg
  stringListArg
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:

  boolArg                   <BOOLEAN>
  Allowed values: true, false

  stringArg                 <TEXT>

  structArg                 <NUMBER>

  structNArg                <NUMBER>

  enumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  objectArg                 <URI>

  stringListArg (Multiple)  <TEXT>
"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:
  structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:

  structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:
  enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:

  enumListArg (Multiple)  <DAYOFWEEK>
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
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:
  objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:

  objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                WhenArgs = "ArgsNoDefault true green 1 2 Monday http://google.com yellow orange",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            BoolArg = true,
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string> {"yellow", "orange"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsNotRequired()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                WhenArgs = "ArgsNoDefault",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            StructArg = default(int),
                            EnumArg = default(DayOfWeek),
                        }
                    }
                }
            });
        }

        [Fact]
        public void StructList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                WhenArgs = "StructListNoDefault 23 5 7",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            StructListArg = new List<int>{23,5,7}
                        }
                    }
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                WhenArgs = "EnumListNoDefault Friday Tuesday Thursday",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            EnumListArg = new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday}
                        }
                    }
                }
            });
        }

        [Fact]
        public void ObjectList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                WhenArgs = "ObjectListNoDefault http://google.com http://apple.com http://github.com",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            ObjectListArg = new List<Uri>
                            {
                                new Uri("http://google.com"),
                                new Uri("http://apple.com"),
                                new Uri("http://github.com"),
                            }
                        }
                    }
                }
            });
        }

        private class OperandsNoDefaults: IArgsNoDefaultsSampleTypesMethod
        {
            private TestCaptures TestCaptures { get; set; }

            public void ArgsNoDefault(
                [Operand] bool boolArg,
                [Operand] string stringArg,
                [Operand] int structArg,
                [Operand] int? structNArg,
                [Operand] DayOfWeek enumArg,
                [Operand] Uri objectArg,
                [Operand] List<string> stringListArg)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListNoDefault(
                [Operand] List<int> structListArg)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListNoDefault(
                [Operand] List<DayOfWeek> enumListArg)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListNoDefault(
                [Operand] List<Uri> objectListArg)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}