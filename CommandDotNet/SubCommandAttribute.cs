using System;

namespace CommandDotNet
{
    /// <summary>
    /// Defines a class as a subcommand of another class.<br/>
    /// Can be used on a property or nested class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubcommandAttribute : Attribute
    {
        /// <summary>
        /// Overrides the command name defined in the <see cref="CommandAttribute"/>
        /// or generated from the class name.
        /// </summary>
        public string? RenameAs { get; set; }
    }

    [Obsolete("use SubcommandAttribute instead. Lowercase C")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SubCommandAttribute : SubcommandAttribute
    {

    }
}