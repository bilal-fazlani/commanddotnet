using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;

public class OperandsDefaultsStructListArgumentModel : IStructListArgumentModel
{
    [Operand]
    public List<int>? StructListArg { get; set; } = [3, 4];
}