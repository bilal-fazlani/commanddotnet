using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class NrtOperandsNoDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Operand]
        public List<Uri>? ObjectListArg { get; set; }
    }
}