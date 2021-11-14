using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class NrtOperandsNoDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        //[Obsolete("no value here")]
        [Operand]
        public bool BoolArg { get; set; }

        [Operand]
        public string? StringArg { get; set; }

        //[Obsolete("no value here")]
        [Operand]
        public int StructArg { get; set; }

        //[Obsolete("no value here")]
        [Operand]
        public int? StructNArg { get; set; }

        //[Obsolete("no value here")]
        [Operand]
        public DayOfWeek EnumArg { get; set; }

        [Operand]
        public Uri? ObjectArg { get; set; }

        [Operand]
        public List<string>? StringListArg { get; set; }
    }
}