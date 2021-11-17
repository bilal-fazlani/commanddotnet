using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    /// <summary>
    /// Named arguments are the <see cref="Option"/>s of a command and
    /// will be referred to as <see cref="Option"/> in the rest of the framework.<br/>
    /// <see cref="Operand"/>s are what the command operates on.<br/>
    /// <see cref="Option"/>s are are how the command operates on the <see cref="Operand"/>s<br/>
    /// https://commanddotnet.bilal-fazlani.com/arguments/option-or-operand/
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class NamedAttribute : OptionAttribute
    {

        /// <summary>
        /// Identifies a property or parameter as an <see cref="Option"/>, aka named argument.
        /// </summary>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure options defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public NamedAttribute(int __callerLineNumber = 0) : base(__callerLineNumber)
        {
        }

        /// <summary>
        /// Identifies a property or parameter as an <see cref="Option"/>, aka named argument.
        /// </summary>
        /// <param name="shortName">The single character short name for the option.</param>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure options defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public NamedAttribute(char shortName, int __callerLineNumber = 0) : base(shortName, __callerLineNumber)
        {
        }

        /// <summary>
        /// Identifies a property or parameter as an <see cref="Option"/>, aka named argument.
        /// </summary>
        /// <param name="longName">
        /// The long name for the option. Defaults to the parameter or property name.<br/>
        /// Set to null to prevent the option from having a long name.</param>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure options defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public NamedAttribute(string? longName, int __callerLineNumber = 0) : base(longName, __callerLineNumber)
        {
        }

        /// <summary>
        /// Identifies a property or parameter as an <see cref="Option"/>, aka named argument.
        /// </summary>
        /// <param name="shortName">The single character short name for the option.</param>
        /// <param name="longName">
        /// The long name for the option. Defaults to the parameter or property name.<br/>
        /// Set to null to prevent the option from having a long name.</param>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure options defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public NamedAttribute(char shortName, string? longName, int __callerLineNumber = 0) : base(shortName, longName, __callerLineNumber)
        {
        }
    }
}