using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsEnumListArgumentModel : IEnumListArgumentModel
    {
        [Argument]
        public List<DayOfWeek> EnumListArg { get; set; }
    }
}