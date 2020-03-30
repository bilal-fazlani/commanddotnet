using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {
        /// <summary>
        /// The single character short name for the option.<br/>
        /// This will change the default behavior of LongName (setting the default to Null) unless
        /// <see cref="AppSettings.LongNameAlwaysDefaultsToSymbolName"/> is true.<br/>
        /// When the setting is true, set <see cref="LongName"/> to null to remove
        /// the default long name.
        /// </summary>
        public string ShortName { get; set; }

        private string _longName;

        /// <summary>
        /// The long name for the option. Defaults to the parameter or property name.<br/>
        /// Set to null to prevent the option from having a long name.
        /// </summary>
        public string LongName
        {
            get => _longName;
            set
            {
                IgnoreDefaultLongName = value.IsNullOrEmpty();
                _longName = value;
            }
        }

        public bool IgnoreDefaultLongName { get; private set; }
        
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