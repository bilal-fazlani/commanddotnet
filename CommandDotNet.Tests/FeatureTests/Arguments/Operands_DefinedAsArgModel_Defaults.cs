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
    public class Operands_DefinedAsArgModel_Defaults : ScenarioTestBase<Operands_DefinedAsArgModel_Defaults>
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsArgModel_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<OperandsDefaults>("SampleTypes - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg

Options:
  -h | --help  Show help information" }
                },
                new Given<OperandsDefaults>("SampleTypes - Detailed Help")
                {
                    SkipReason = "Help should not contain default value as [System.Collections.Generic.List`1[System.String]]",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ArgsDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:

  BoolArg                     <BOOLEAN>      [True]
  Allowed values: true, false

  StringArg                   <TEXT>         [lala]

  StructArg                   <NUMBER>       [3]

  StructNArg                  <NUMBER>       [4]

  EnumArg                     <DAYOFWEEK>    [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                   <URI>          [http://google.com/]

  StringListArg (Multiple)    <TEXT>         [System.Collections.Generic.List`1[System.String]]


Options:

  -h | --help
  Show help information" }
                },
                new Given<OperandsDefaults>("StructList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "StructListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:
  StructListArg

Options:
  -h | --help  Show help information" }
                },
                new Given<OperandsDefaults>("StructList - Detailed Help")
                {
                    SkipReason = "Help should not contain default value as [System.Collections.Generic.List`1[System.Int32]]",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "StructListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:

  StructListArg (Multiple)    <NUMBER>    [System.Collections.Generic.List`1[System.Int32]]


Options:

  -h | --help
  Show help information" }
                },
                new Given<OperandsDefaults>("EnumList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "EnumListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:
  EnumListArg

Options:
  -h | --help  Show help information" }
                },
                new Given<OperandsDefaults>("EnumList - Detailed Help")
                {
                    SkipReason = "Help should not contain default value as [System.Collections.Generic.List`1[System.DayOfWeek]]",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "EnumListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:

  EnumListArg (Multiple)    <DAYOFWEEK>    [System.Collections.Generic.List`1[System.DayOfWeek]]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday


Options:

  -h | --help
  Show help information" }
                },
                new Given<OperandsDefaults>("ObjectList - Basic Help")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "ObjectListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:
  ObjectListArg

Options:
  -h | --help  Show help information" }
                },
                new Given<OperandsDefaults>("ObjectList - Detailed Help")
                {
                    SkipReason = "Help should not contain default value as [System.Collections.Generic.List`1[System.Uri]]",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "ObjectListDefaults -h",
                    Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:

  ObjectListArg (Multiple)    <URI>    [System.Collections.Generic.List`1[System.Uri]]


Options:

  -h | --help
  Show help information" }
                },
                new Given<OperandsDefaults>("SampleTypes - Exec - positional")
                {
                    WhenArgs = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange",
                    Then =
                    {
                        Outputs = { new OperandsDefaultsSampleTypesModel
                        {
                            BoolArg = true,
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string>{"yellow", "orange"}
                        } }
                    }
                },
                new Given<OperandsDefaults>("SampleTypes - Exec - operands not required - uses defaults")
                {
                    WhenArgs = "ArgsDefaults",
                    Then =
                    {
                        Outputs =
                        {
                            new OperandsDefaultsSampleTypesModel
                            {
                                StringArg = "lala",
                                StructArg = 3,
                                StructNArg = 4,
                                EnumArg = DayOfWeek.Tuesday,
                            }
                        }
                    }
                },
            };

        private class OperandsDefaults
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(OperandsDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }

            public void StructListDefaults(OperandsDefaultsStructListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void EnumListDefaults(OperandsDefaultsEnumListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void ObjectListDefaults(OperandsDefaultsObjectListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}