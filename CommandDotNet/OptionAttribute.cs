using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {
        /// <summary>Must be a single character</summary>
        public string ShortName { get; set; }

        public string LongName { get; set; }

        string INameAndDescription.Name => LongName;

        /// <summary>
        /// The <see cref="BooleanMode"/> to use for this option.
        /// If not specified, the default from <see cref="AppSettings"/> will be used.
        /// </summary>
        public BooleanMode BooleanMode { get; set; }

        [Obsolete("Use AssignToExecutableSubcommands instead")]
        public bool Inherited
        {
            get => AssignToExecutableSubcommands;
            set => AssignToExecutableSubcommands = value;
        }

        /// <summary>
        /// When true, this option is an Inherited option, defined by a command interceptor method
        /// and assigned to executable subcommands of the interceptor.<br/>
        /// Note: The Parent will still be the defining command, not the target command.
        /// </summary>
        public bool AssignToExecutableSubcommands { get; set; }

        public string Description { get; set; }
    }
}