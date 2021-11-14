using System.Collections.Generic;
#pragma warning disable 8767

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsStructListArgumentModel : IStructListArgumentModel
    {
        [Operand]
        public List<int> StructListArg { get; set; } = null!;
    }
}