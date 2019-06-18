using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsEnumListArgumentModel : IEnumListArgumentModel
    {
        [Operand]
        public List<DayOfWeek> EnumListArg { get; set; } = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday };
    }
}