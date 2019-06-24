using System.Collections.Generic;

namespace CommandDotNet
{
    internal interface ISettableArgument: IArgument
    {
        void SetValues(List<string> values);
    }
}