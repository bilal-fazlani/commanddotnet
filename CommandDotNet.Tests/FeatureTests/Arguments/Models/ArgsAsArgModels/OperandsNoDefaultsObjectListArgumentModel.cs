using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Argument]
        public List<Uri> ObjectListArg { get; set; }
    }
}