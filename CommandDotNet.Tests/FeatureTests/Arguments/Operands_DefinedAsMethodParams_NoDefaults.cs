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
    public class Operands_DefinedAsMethodParams_NoDefaults : ScenarioTestBase<Operands_DefinedAsMethodParams_NoDefaults>
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_NoDefaults(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OperandsNoDefaults>("SampleTypes - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ArgsNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments] [options]

Arguments:
  boolArg
  stringArg
  structArg
  structNArg
  enumArg
  objectArg
  stringListArg

Options:
  -h | --help  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("SampleTypes - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ArgsNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ArgsNoDefault [arguments] [options]

Arguments:

  boolArg                     <BOOLEAN>
  Allowed values: true, false

  stringArg                   <TEXT>

  structArg                   <NUMBER>

  structNArg                  <NUMBER>

  enumArg                     <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  objectArg                   <URI>

  stringListArg (Multiple)    <TEXT>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("StructList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "StructListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListNoDefault [arguments] [options]

Arguments:
  structListArg

Options:
  -h | --help  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("StructList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "StructListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll StructListNoDefault [arguments] [options]

Arguments:

  structListArg (Multiple)    <NUMBER>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("EnumList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "EnumListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments] [options]

Arguments:
  enumListArg

Options:
  -h | --help  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("EnumList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "EnumListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll EnumListNoDefault [arguments] [options]

Arguments:

  enumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("ObjectList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ObjectListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments] [options]

Arguments:
  objectListArg

Options:
  -h | --help  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("ObjectList - Detailed Help")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ObjectListNoDefault -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ObjectListNoDefault [arguments] [options]

Arguments:

  objectListArg (Multiple)    <URI>


Options:

  -h | --help
  Show help information"
                    }
                },
                new Given<OperandsNoDefaults>("SampleTypes - Exec - positional")
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
                },
                new Given<OperandsNoDefaults>("SampleTypes - Exec - operands not required")
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
                new Given<OperandsNoDefaults>("StructList - Exec - positional")
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
                },
                new Given<OperandsNoDefaults>("EnumList - Exec - positional")
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
                },
                new Given<OperandsNoDefaults>("ObjectList - Exec - positional")
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
                },
            };

        private class OperandsNoDefaults: IArgsNoDefaultsSampleTypesMethod
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsNoDefault(
                [Argument] bool boolArg,
                [Argument] string stringArg,
                [Argument] int structArg,
                [Argument] int? structNArg,
                [Argument] DayOfWeek enumArg,
                [Argument] Uri objectArg,
                [Argument] List<string> stringListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListNoDefault(
                [Argument] List<int> structListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListNoDefault(
                [Argument] List<DayOfWeek> enumListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListNoDefault(
                [Argument] List<Uri> objectListArg)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}