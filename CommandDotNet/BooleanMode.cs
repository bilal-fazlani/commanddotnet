using System;

namespace CommandDotNet
{
    /// <summary>
    /// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
    /// When Implicit, boolean options are treated as Flags, considered false unless it's specified
    /// and the next argument will be considered a new argument.
    /// </summary>
    public enum BooleanMode
    {
        [Obsolete("Use either Implicit or Explicit or Nullable<BooleanMode>")]
        Unknown = 0,
        Implicit = 1,
        Explicit = 2
    }
}