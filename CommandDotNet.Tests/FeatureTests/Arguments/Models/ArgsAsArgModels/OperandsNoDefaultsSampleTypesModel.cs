using System;
using System.Collections.Generic;
#pragma warning disable 8767

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        [Operand]
        public bool BoolArg { get; set; }

        [Operand]
        public string StringArg { get; set; } = null!;

        [Operand]
        public int StructArg { get; set; }

        [Operand]
        public int? StructNArg { get; set; }

        [Operand]
        public DayOfWeek EnumArg { get; set; }

        [Operand]
        public Uri ObjectArg { get; set; } = null!;

        [Operand]
        public List<string> StringListArg { get; set; } = null!;
    }
}