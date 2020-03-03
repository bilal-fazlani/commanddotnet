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
    public class Options_DefinedAsMethodParams_NoDefaults_Tests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_NoDefaults_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

Options:
  --boolArg
  --stringArg
  --structArg
  --structNArg
  --enumArg
  --objectArg
  --stringListArg"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

Options:

  --boolArg

  --stringArg                 <TEXT>

  --structArg                 <NUMBER>

  --structNArg                <NUMBER>

  --enumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                 <URI>

  --stringListArg (Multiple)  <TEXT>"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:
  --structListArg"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "StructListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:

  --structListArg (Multiple)  <NUMBER>"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

Options:
  --enumListArg"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

Options:

  --enumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:
  --objectListArg"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListNoDefault -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:

  --objectListArg (Multiple)  <URI>"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                WhenArgs = "ArgsNoDefault --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
                           "--objectArg http://google.com --stringListArg yellow --stringListArg orange",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
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
            Verify(new Scenario<OptionsNoDefaults>
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
        public void StructList_Exec_Named()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                WhenArgs = "StructListNoDefault --structListArg 23 --structListArg 5 --structListArg 7",
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
        public void EnumList_Exec_Named()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                WhenArgs = "EnumListNoDefault --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday",
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
        public void ObjectList_Exec_Named()
        {
            Verify(new Scenario<OptionsNoDefaults>
            {
                WhenArgs = "ObjectListNoDefault --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com",
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

        private class OptionsNoDefaults : IArgsNoDefaultsSampleTypesMethod
        {
            private TestOutputs TestOutputs { get; set; }

            public void ArgsNoDefault(
                [Option] bool boolArg,
                [Option] string stringArg,
                [Option] int structArg,
                [Option] int? structNArg,
                [Option] DayOfWeek enumArg,
                [Option] Uri objectArg,
                [Option] List<string> stringListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListNoDefault(
                [Option] List<int> structListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListNoDefault(
                [Option] List<DayOfWeek> enumListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListNoDefault(
                [Option] List<Uri> objectListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}