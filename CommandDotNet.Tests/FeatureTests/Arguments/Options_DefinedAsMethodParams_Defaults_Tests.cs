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
    public class Options_DefinedAsMethodParams_Defaults_Tests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_Defaults_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

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
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --boolArg                                [True]

  --stringArg                 <TEXT>       [lala]

  --structArg                 <NUMBER>     [3]

  --structNArg                <NUMBER>     [4]

  --enumArg                   <DAYOFWEEK>  [Wednesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                 <URI>

  --stringListArg (Multiple)  <TEXT>"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:
  --structListArg"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:

  --structListArg (Multiple)  <NUMBER>"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:
  --enumListArg"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:

  --enumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:
  --objectListArg"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:

  --objectListArg (Multiple)  <URI>"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
                           "--objectArg http://google.com --stringListArg yellow --stringListArg orange",
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
        public void SampleTypes_Exec_OptionsNotRequired_UsesDefaults()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults",
                Then =
                {
                    Captured =
                    {
                        new ParametersSampleTypesResults
                        {
                            BoolArg = true,
                            StringArg = "lala",
                            StructArg = 3,
                            StructNArg = 4,
                            EnumArg = DayOfWeek.Wednesday,
                        }
                    }
                }
            });
        }

        [Fact]
        public void StructList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "StructListDefaults --structListArg 23 --structListArg 5 --structListArg 7",
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
        public void EnumList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "EnumListDefaults --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday",
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
        public void ObjectList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "ObjectListDefaults --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com",
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

        private class OptionsDefaults : IArgsDefaultsSampleTypesMethod
        {
            private TestCaptures TestCaptures { get; set; }

            public void ArgsDefaults(
                [Option] bool boolArg = true,
                [Option] string stringArg = "lala", 
                [Option] int structArg = 3, 
                [Option] int? structNArg = 4,
                [Option] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Option] Uri objectArg = null,
                [Option] List<string> stringListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Option] List<int> structListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Option] List<DayOfWeek> enumListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Option] List<Uri> objectListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}