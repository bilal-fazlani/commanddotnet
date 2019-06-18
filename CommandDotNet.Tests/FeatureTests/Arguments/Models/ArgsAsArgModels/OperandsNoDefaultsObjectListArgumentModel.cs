using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Operand]
        public List<Uri> ObjectListArg { get; set; }
    }
}