using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;

public class OperandsDefaultsEnumListArgumentModel : IEnumListArgumentModel
{
    [Operand]
    public List<DayOfWeek>? EnumListArg { get; set; } = [DayOfWeek.Monday, DayOfWeek.Tuesday];
}