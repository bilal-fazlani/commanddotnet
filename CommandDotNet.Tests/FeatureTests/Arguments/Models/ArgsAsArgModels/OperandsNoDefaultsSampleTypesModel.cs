using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        [Argument]
        public string StringArg { get; set; }

        [Argument]
        public int StructArg { get; set; }

        [Argument]
        public int? StructNArg { get; set; }

        [Argument]
        public DayOfWeek EnumArg { get; set; }

        [Argument]
        public Uri ObjectArg { get; set; }

        [Argument]
        public List<string> StringListArg { get; set; }
    }
}