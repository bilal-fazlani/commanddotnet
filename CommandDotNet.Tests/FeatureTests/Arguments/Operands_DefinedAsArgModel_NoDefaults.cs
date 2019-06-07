using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsArgModel_NoDefaults : ScenarioTestBase<Operands_DefinedAsArgModel_NoDefaults>
    {
        private static AppSettings BasicHelp = new AppSettings{Help = {TextStyle = HelpTextStyle.Basic}};
        private static AppSettings DetailedHelp = new AppSettings {Help = {TextStyle = HelpTextStyle.Detailed}};

        public Operands_DefinedAsArgModel_NoDefaults(ITestOutputHelper output) : base(output)
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
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg

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

  BoolArg                     <BOOLEAN>
  Allowed values: true, false

  StringArg                   <TEXT>

  StructArg                   <NUMBER>

  StructNArg                  <NUMBER>

  EnumArg                     <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                   <URI>

  StringListArg (Multiple)    <TEXT>


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
  StructListArg

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

  StructListArg (Multiple)    <NUMBER>


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
  EnumListArg

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

  EnumListArg (Multiple)    <DAYOFWEEK>
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
  ObjectListArg

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

  ObjectListArg (Multiple)    <URI>


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
                            new OperandsNoDefaultsSampleTypesModel
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
                            new OperandsNoDefaultsSampleTypesModel
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
                            new OperandsNoDefaultsStructListArgumentModel
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
                            new OperandsNoDefaultsEnumListArgumentModel
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
                            new OperandsNoDefaultsObjectListArgumentModel
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

        private class OperandsNoDefaults
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsNoDefault(OperandsNoDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }

            public void StructListNoDefault(OperandsNoDefaultsStructListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void EnumListNoDefault(OperandsNoDefaultsEnumListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void ObjectListNoDefault(OperandsNoDefaultsObjectListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}