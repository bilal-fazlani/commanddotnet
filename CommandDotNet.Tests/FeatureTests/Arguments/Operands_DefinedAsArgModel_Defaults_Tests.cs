using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsArgModel_Defaults_Tests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsArgModel_Defaults_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp_IncludesAll()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg" }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp_IncludesAll()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

Arguments:

  BoolArg                   <BOOLEAN>    [True]
  Allowed values: true, false

  StringArg                 <TEXT>       [lala]

  StructArg                 <NUMBER>     [3]

  StructNArg                <NUMBER>     [4]

  EnumArg                   <DAYOFWEEK>  [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                 <URI>        [http://google.com/]

  StringListArg (Multiple)  <TEXT>       [red, blue]" }
            });
        }

        [Fact]
        public void StructList_BasicHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:
  StructListArg" }
            });
        }

        [Fact]
        public void StructList_DetailedHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:

  StructListArg (Multiple)  <NUMBER>  [3, 4]" }
            });
        }

        [Fact]
        public void EnumList_BasicHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

Arguments:
  EnumListArg" }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

Arguments:

  EnumListArg (Multiple)  <DAYOFWEEK>  [Monday, Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday" }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:
  ObjectListArg" }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp_IncludesList()
        {
            Verify(new Scenario<OperandsDefaults>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:

  ObjectListArg (Multiple)  <URI>  [http://google.com/, http://github.com/]" }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsAreAssignedByPosition()
        {
            Verify(new Scenario<OperandsDefaults>
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
            });
        }

        [Fact]
        public void SampleType_Exec_OperandsAreNotRequired_UsesDefaults()
        {
            Verify(new Scenario<OperandsDefaults>
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
            });
        }

        private class OperandsDefaults
        {
            private TestOutputs TestOutputs { get; set; }

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