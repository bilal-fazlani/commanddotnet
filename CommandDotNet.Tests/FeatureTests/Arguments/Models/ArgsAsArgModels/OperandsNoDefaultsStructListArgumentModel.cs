using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsStructListArgumentModel : IStructListArgumentModel
    {
        [Argument]
        public List<int> StructListArg { get; set; }
    }
}