using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams
{
    public interface IArgsNoDefaultsSampleTypesMethod
    {
        void ArgsNoDefault(
            bool boolArg,
            string stringArg,
            int structArg,
            int? structNArg,
            DayOfWeek enumArg,
            Uri objectArg,
            List<string> stringListArg);

        void StructListNoDefault(List<int> structListArg);
        void EnumListNoDefault(List<DayOfWeek> enumListArg);
        void ObjectListNoDefault(List<Uri> objectListArg);
    }
}