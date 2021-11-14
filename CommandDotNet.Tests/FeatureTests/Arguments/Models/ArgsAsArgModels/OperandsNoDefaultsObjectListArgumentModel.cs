using System;
using System.Collections.Generic;
#pragma warning disable 8767

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Operand]
        public List<Uri> ObjectListArg { get; set; } = null!;
    }
}