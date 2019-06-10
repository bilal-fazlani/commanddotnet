using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsMethodParams_NoDefaults : ScenarioTestBase<Options_DefinedAsMethodParams_NoDefaults>
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_NoDefaults(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OptionsNoDefaults>("SampleTypes - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ArgsNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

Options:
  -h | --help      Show help information
  --boolArg
  --stringArg
  --structArg
  --structNArg
  --enumArg
  --objectArg
  --stringListArg"
                    }
                },
                new Given<OptionsNoDefaults>("SampleTypes - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ArgsNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

Options:

  -h | --help
  Show help information

  --boolArg

  --stringArg                   <TEXT>

  --structArg                   <NUMBER>

  --structNArg                  <NUMBER>

  --enumArg                     <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                   <URI>

  --stringListArg (Multiple)    <TEXT>"
                    }
                },
                new Given<OptionsNoDefaults>("StructList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "StructListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:
  -h | --help      Show help information
  --structListArg"
                    }
                },
                new Given<OptionsNoDefaults>("StructList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "StructListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:

  -h | --help
  Show help information

  --structListArg (Multiple)    <NUMBER>"
                    }
                },
                new Given<OptionsNoDefaults>("EnumList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "EnumListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

Options:
  -h | --help    Show help information
  --enumListArg"
                    }
                },
                new Given<OptionsNoDefaults>("EnumList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "EnumListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

Options:

  -h | --help
  Show help information

  --enumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
                    }
                },
                new Given<OptionsNoDefaults>("ObjectList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ObjectListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:
  -h | --help      Show help information
  --objectListArg"
                    }
                },
                new Given<OptionsNoDefaults>("ObjectList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ObjectListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:

  -h | --help
  Show help information

  --objectListArg (Multiple)    <URI>"
                    }
                },
                new Given<OptionsNoDefaults>("SampleTypes - Exec - named")
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
                },
                new Given<OptionsNoDefaults>("SampleTypes - Exec - operands not required")
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
                },
                new Given<OptionsNoDefaults>("StructList - Exec - named")
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
                },
                new Given<OptionsNoDefaults>("EnumList - Exec - named")
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
                },
                new Given<OptionsNoDefaults>("ObjectList - Exec - named")
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
                },
            };

        private class OptionsNoDefaults : IArgsNoDefaultsSampleTypesMethod
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

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