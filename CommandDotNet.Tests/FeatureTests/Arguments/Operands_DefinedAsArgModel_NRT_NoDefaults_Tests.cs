using System;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments;

public class Operands_DefinedAsArgModel_NRT_NoDefaults_Tests
{
    private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
    private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

    public Operands_DefinedAsArgModel_NRT_NoDefaults_Tests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void SampleTypes_Argument_Default_is_null()
    {
        new AppRunner<OperandsNoDefaults>().Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ArgsNoDefault)} -h" },
            Then =
            {
                AssertContext = ctx =>
                {
                    var cmd = ctx.GetCommandInvocationInfo().Command;
                    cmd.Find<IArgument>("BoolArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("StringArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("StructArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("StructNArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("EnumArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("ObjectArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("ObjectArg")!.Arity.Minimum.Should().Be(0);
                    cmd.Find<IArgument>("StringListArg")!.Default.Should().BeNull();
                    cmd.Find<IArgument>("StringListArg")!.Arity.Minimum.Should().Be(0);
                }
            }
        });
    }

    [Fact]
    public void SampleTypes_BasicHelp()
    {
        new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ArgsNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll ArgsNoDefault <BoolArg> <StringArg> <StructArg> <StructNArg> <EnumArg> [<ObjectArg> <StringListArg>]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg"
            }
        });
    }

    [Fact]
    public void SampleTypes_DetailedHelp()
    {
        new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ArgsNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll ArgsNoDefault <BoolArg> <StringArg> <StructArg> <StructNArg> <EnumArg> [<ObjectArg> <StringListArg>]

Arguments:

  BoolArg                   <BOOLEAN>
  Allowed values: true, false

  StringArg                 <TEXT>

  StructArg                 <NUMBER>

  StructNArg                <NUMBER>

  EnumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                 <URI>

  StringListArg (Multiple)  <TEXT>"
            }
        });
    }

    [Fact]
    public void StructList_BasicHelp()
    {
        new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.StructListNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll StructListNoDefault [<StructListArg>]

Arguments:
  StructListArg"
            }
        });
    }

    [Fact]
    public void StructList_DetailedHelp()
    {
        new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.StructListNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll StructListNoDefault [<StructListArg>]

Arguments:

  StructListArg (Multiple)  <NUMBER>"
            }
        });
    }

    [Fact]
    public void EnumList_BasicHelp()
    {
        new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.EnumListNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll EnumListNoDefault [<EnumListArg>]

Arguments:
  EnumListArg"
            }
        });
    }

    [Fact]
    public void EnumList_DetailedHelp()
    {
        new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.EnumListNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll EnumListNoDefault [<EnumListArg>]

Arguments:

  EnumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday"
            }
        });
    }

    [Fact]
    public void ObjectList_BasicHelp()
    {
        new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ObjectListNoDefault)} -h" },
            Then =
            {
                Output = @"Usage: testhost.dll ObjectListNoDefault [<ObjectListArg>]

Arguments:
  ObjectListArg"
            }
        });
    }

    [Fact]
    public void ObjectList_DetailedHelp()
    {
        new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ObjectListNoDefault)}  -h" },
            Then =
            {
                Output = @"Usage: testhost.dll ObjectListNoDefault [<ObjectListArg>]

Arguments:

  ObjectListArg (Multiple)  <URI>"
            }
        });
    }

    [Fact]
    public void SampleTypes_Exec_Positional()
    {
        new AppRunner<OperandsNoDefaults>().Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ArgsNoDefault)} true green 1 2 Monday http://google.com yellow orange" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(
                    new NrtOperandsNoDefaultsSampleTypesModel
                    {
                        BoolArg = true,
                        StringArg = "green",
                        StructArg = 1,
                        StructNArg = 2,
                        EnumArg = DayOfWeek.Monday,
                        ObjectArg = new Uri("http://google.com"),
                        StringListArg = ["yellow", "orange"]
                    })
            }
        });
    }

    [Fact]
    public void StructList_Exec_Positional()
    {
        new AppRunner<OperandsNoDefaults>().Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.StructListNoDefault)} 23 5 7" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(
                    new NrtOperandsNoDefaultsStructListArgumentModel
                    {
                        StructListArg = [23, 5, 7]
                    })
            }
        });
    }

    [Fact]
    public void EnumList_Exec_Positional()
    {
        new AppRunner<OperandsNoDefaults>().Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.EnumListNoDefault)} Friday Tuesday Thursday" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(
                    new NrtOperandsNoDefaultsEnumListArgumentModel
                    {
                        EnumListArg = [DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday]
                    })
            }
        });
    }

    [Fact]
    public void ObjectList_Exec_Positional()
    {
        new AppRunner<OperandsNoDefaults>().Verify(new Scenario
        {
            When = { Args = $"{nameof(OperandsNoDefaults.ObjectListNoDefault)} http://google.com http://apple.com http://github.com" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(
                    new NrtOperandsNoDefaultsObjectListArgumentModel
                    {
                        ObjectListArg =
                        [
                            new("http://google.com"),
                            new("http://apple.com"),
                            new("http://github.com")
                        ]
                    })
            }
        });
    }

    [UsedImplicitly]
    private class OperandsNoDefaults
    {
        public void ArgsNoDefault(NrtOperandsNoDefaultsSampleTypesModel model)
        {
        }

        public void StructListNoDefault(NrtOperandsNoDefaultsStructListArgumentModel model)
        {
        }

        public void EnumListNoDefault(NrtOperandsNoDefaultsEnumListArgumentModel model)
        {
        }

        public void ObjectListNoDefault(NrtOperandsNoDefaultsObjectListArgumentModel model)
        {
        }
    }
}