using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsMethodParams_Defaults : ScenarioTestBase<Options_DefinedAsMethodParams_Defaults>
    {
        private static AppSettings BasicHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Basic } };
        private static AppSettings DetailedHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed } };

        public Options_DefinedAsMethodParams_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OptionsDefaults>("SampleTypes - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

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
                new Given<OptionsDefaults>("SampleTypes - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  -h | --help
  Show help information

  --boolArg                                    [True]

  --stringArg                   <TEXT>         [lala]

  --structArg                   <NUMBER>       [3]

  --structNArg                  <NUMBER>       [4]

  --enumArg                     <DAYOFWEEK>    [Wednesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                   <URI>

  --stringListArg (Multiple)    <TEXT>"
                    }
                },
                new Given<OptionsDefaults>("StructList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "StructListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:
  -h | --help      Show help information
  --structListArg"
                    }
                },
                new Given<OptionsDefaults>("StructList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "StructListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:

  -h | --help
  Show help information

  --structListArg (Multiple)    <NUMBER>"
                    }
                },
                new Given<OptionsDefaults>("EnumList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "EnumListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:
  -h | --help    Show help information
  --enumListArg"
                    }
                },
                new Given<OptionsDefaults>("EnumList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "EnumListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:

  -h | --help
  Show help information

  --enumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
                    }
                },
                new Given<OptionsDefaults>("ObjectList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ObjectListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:
  -h | --help      Show help information
  --objectListArg"
                    }
                },
                new Given<OptionsDefaults>("ObjectList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ObjectListDefaults -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:

  -h | --help
  Show help information

  --objectListArg (Multiple)    <URI>"
                    }
                },
                new Given<OptionsDefaults>("SampleTypes - Exec - named")
                {
                    WhenArgs = "ArgsDefaults --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
                               "--objectArg http://google.com --stringListArg yellow --stringListArg orange",
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
                },
                new Given<OptionsDefaults>("SampleTypes - Exec - options not required - uses defaults")
                {
                    WhenArgs = "ArgsDefaults",
                    Then =
                    {
                        Outputs =
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
                },
                new Given<OptionsDefaults>("StructList - Exec - named")
                {
                    WhenArgs = "StructListDefaults --structListArg 23 --structListArg 5 --structListArg 7",
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
                new Given<OptionsDefaults>("EnumList - Exec - named")
                {
                    WhenArgs = "EnumListDefaults --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday",
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
                new Given<OptionsDefaults>("ObjectList - Exec - named")
                {
                    WhenArgs = "ObjectListDefaults --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com",
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

        private class OptionsDefaults : IArgsDefaultsSampleTypesMethod
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(
                [Option] bool boolArg = true,
                [Option] string stringArg = "lala", 
                [Option] int structArg = 3, 
                [Option] int? structNArg = 4,
                [Option] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Option] Uri objectArg = null,
                [Option] List<string> stringListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Option] List<int> structListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Option] List<DayOfWeek> enumListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Option] List<Uri> objectListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}