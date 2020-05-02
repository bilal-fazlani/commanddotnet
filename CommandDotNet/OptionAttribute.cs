using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {
        /// <summary>
        /// The single character short name for the option.<br/>
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
                if (value == string.Empty)
                {
                    value = null;
                }
                IgnoreDefaultLongName = value == null;
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

        /// <summary>
        /// When true, this option is an Inherited option, defined by a command interceptor method
        /// and assigned to executable subcommands of the interceptor.<br/>
        /// Note: The Parent will still be the defining command, not the target command.
        /// </summary>
        public bool AssignToExecutableSubcommands { get; set; }

        public string Description { get; set; }

        public int CallerLineNumber { get; }

        /// <summary>
        /// Constructs an <see cref="OptionAttribute"/>
        /// </summary>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure options defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public OptionAttribute([CallerLineNumber] int __callerLineNumber = 0)
        {
            CallerLineNumber = __callerLineNumber;
        }
    }
}