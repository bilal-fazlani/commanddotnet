using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Argument]
        public List<Uri> ObjectListArg { get; set; } = new List<Uri>
        {
            new Uri("http://google.com"),
            new Uri("http://github.com")
        };
    }
}