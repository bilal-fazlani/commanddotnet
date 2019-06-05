using System;
using System.Collections.Generic;

public interface IArgsNoDefaultsSampleTypesMethod
{
    void ArgsNoDefault(
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