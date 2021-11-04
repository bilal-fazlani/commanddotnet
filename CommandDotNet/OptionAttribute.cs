using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    /// <summary>
    /// <see cref="Option"/>s are the named arguments of a command.<br/>
    /// <see cref="Operand"/>s are what the command operates on.<br/>
    /// <see cref="Option"/>s are are how the command operates on the <see cref="Operand"/>s<br/>
    /// https://commanddotnet.bilal-fazlani.com/arguments/option-or-operand/
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OptionAttribute : Attribute, INameAndDescription
    {
        /// <summary>
        /// The single character short name for the option.<br/>
        /// </summary>
        public string? ShortName { get; set; }

        private string? _longName;

        /// <summary>
        /// The long name for the option. Defaults to the parameter or property name.<br/>
        /// Set to null to prevent the option from having a long name.
        /// </summary>
        public string? LongName
        {
            get => _longName;
            set
            {
                if (value == string.Empty)
                {
                    value = null;
                }
                NoLongName = value == null;
                _longName = value;
            }
        }

        /// <summary>
        /// True when <see cref="LongName"/> is explicitly set to null.<br/>
        /// When true, the option will not have a long name and <see cref="ShortName"/> must be set.
        /// </summary>
        public bool NoLongName { get; private set; }
        
        string? INameAndDescription.Name => LongName;

        public string? Description { get; set; }

        /// <summary>
        /// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
        /// When Implicit, boolean options are treated as Flags, considered false unless it's specified
        /// and the next argument will be considered a new argument.<br/>
        /// If not specified, the default from <see cref="AppSettings"/> will be used.
        /// </summary>
        public BooleanMode BooleanMode
        {
            get => BooleanModeAsNullable.GetValueOrDefault();
            set => BooleanModeAsNullable = value;
        }

        internal BooleanMode? BooleanModeAsNullable { get; private set; }

        /// <summary>
        /// When true, this option is an Inherited option, defined by a command interceptor method
        /// and assigned to executable subcommands of the interceptor.<br/>
        /// Note: The Parent will still be the defining command, not the target command.
        /// </summary>
        public bool AssignToExecutableSubcommands { get; set; }

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