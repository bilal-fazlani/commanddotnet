using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        [Argument]
        public string StringArg { get; set; } = "lala";

        [Argument]
        public int StructArg { get; set; } = 3;

        [Argument]
        public int? StructNArg { get; set; } = 4;

        [Argument]
        public DayOfWeek EnumArg { get; set; } = DayOfWeek.Tuesday;

        [Argument]
        public Uri ObjectArg { get; set; } = new Uri("http://google.com");

        [Argument]
        public List<string> StringListArg { get; set; } = new List<string> {"red", "blue"};
    }
}