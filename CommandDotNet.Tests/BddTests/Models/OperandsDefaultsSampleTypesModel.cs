using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.BddTests.Models
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

        [Argument]
        public List<int> StructListArg { get; set; } = new List<int> {3, 4};

        [Argument]
        public List<DayOfWeek> EnumListArg { get; set; } = new List<DayOfWeek> {DayOfWeek.Monday, DayOfWeek.Tuesday};

        [Argument]
        public List<Uri> ObjectListArg { get; set; } = new List<Uri>
        {
            new Uri("http://google.com"),
            new Uri("http://github.com")
        };
    }
}