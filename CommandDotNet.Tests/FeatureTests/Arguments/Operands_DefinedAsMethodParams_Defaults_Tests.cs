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
    public class Operands_DefinedAsMethodParams_Defaults_Tests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_Defaults_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

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
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [arguments]

Arguments:

  boolArg                   <BOOLEAN>    [True]
  Allowed values: true, false

  stringArg                 <TEXT>       [lala]

  structArg                 <NUMBER>     [3]

  structNArg                <NUMBER>     [4]

  enumArg                   <DAYOFWEEK>  [Wednesday]
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
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:
  structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [arguments]

Arguments:

  structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

Arguments:
  enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [arguments]

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
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:
  objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments]

Arguments:

  objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            new AppRunner<OperandsDefaults>().Verify(_output, new Scenario
            {
                WhenArgs = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange",
                Then =
                {
                    Captured = { new ParametersSampleTypesResults
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
        public void SampleTypes_Exec_OperandsNotRequired_UsesDefaults()
        {
            new AppRunner<OperandsDefaults>().Verify(_output, new Scenario
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

        private class OperandsDefaults : IArgsDefaultsSampleTypesMethod
        {
            private TestCaptures TestCaptures { get; set; }

            public void ArgsDefaults(
                [Operand] bool boolArg = true,
                [Operand] string stringArg = "lala", 
                [Operand] int structArg = 3, 
                [Operand] int? structNArg = 4,
                [Operand] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Operand] Uri objectArg = null,
                [Operand] List<string> stringListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Operand] List<int> structListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Operand] List<DayOfWeek> enumListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Operand] List<Uri> objectListArg = null)
            {
                TestCaptures.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}