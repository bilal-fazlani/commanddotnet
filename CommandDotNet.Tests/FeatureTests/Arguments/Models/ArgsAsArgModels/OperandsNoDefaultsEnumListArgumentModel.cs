using System;
using System.Collections.Generic;
#pragma warning disable 8767

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsEnumListArgumentModel : IEnumListArgumentModel
    {
        [Operand]
        public List<DayOfWeek> EnumListArg { get; set; } = null!;
    }
}