using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsMethodParams_NoDefaults_Tests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_NoDefaults_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:
  boolArg
  stringArg
  structArg
  structNArg
  enumArg
  objectArg
  stringListArg"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments]

Arguments:

  boolArg                   <BOOLEAN>
  Allowed values: true, false

  stringArg                 <TEXT>

  structArg                 <NUMBER>

  structNArg                <NUMBER>

  enumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  objectArg                 <URI>

  stringListArg (Multiple)  <TEXT>"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:
  structListArg"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListNoDefault [arguments]

Arguments:

  structListArg (Multiple)  <NUMBER>"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:
  enumListArg"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments]

Arguments:

  enumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:
  objectListArg"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments]

Arguments:

  objectListArg (Multiple)  <URI>"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            Verify(new Scenario<OperandsNoDefaults>
            {
                WhenArgs = "ArgsNoDefault true green 1 2 Monday http://google.com yellow orange",
                Then =
                {
                    Outputs =
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
            Verify(new Scenario<OperandsNoDefaults>
            {
                WhenArgs = "ArgsNoDefault",
                Then =
                {
                    Outputs =
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
            Verify(new Scenario<OperandsNoDefaults>
            {
                WhenArgs = "StructListNoDefault 23 5 7",
                Then =
                {
                    Outputs =
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
            Verify(new Scenario<OperandsNoDefaults>
            {
                WhenArgs = "EnumListNoDefault Friday Tuesday Thursday",
                Then =
                {
                    Outputs =
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
            Verify(new Scenario<OperandsNoDefaults>
            {
                WhenArgs = "ObjectListNoDefault http://google.com http://apple.com http://github.com",
                Then =
                {
                    Outputs =
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
            private TestOutputs TestOutputs { get; set; }

            public void ArgsNoDefault(
                [Operand] bool boolArg,
                [Operand] string stringArg,
                [Operand] int structArg,
                [Operand] int? structNArg,
                [Operand] DayOfWeek enumArg,
                [Operand] Uri objectArg,
                [Operand] List<string> stringListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListNoDefault(
                [Operand] List<int> structListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListNoDefault(
                [Operand] List<DayOfWeek> enumListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListNoDefault(
                [Operand] List<Uri> objectListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}