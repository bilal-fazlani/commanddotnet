using System;

namespace CommandDotNet
{
    public enum BooleanMode
    {
        [Obsolete("Use either Implicit or Explicit or Nullable<BooleanMode>")]
        Unknown = 0,
        Implicit = 1,
        Explicit = 2
    }
}