using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class NrtOperandsNoDefaultsStructListArgumentModel : IStructListArgumentModel
    {
        [Operand]
        public List<int>? StructListArg { get; set; }
    }
}