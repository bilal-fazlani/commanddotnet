using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    /// <summary>
    /// Positional arguments are the <see cref="Operand"/>s of a command and
    /// will be referred to as <see cref="Operand"/> in the rest of the framework.<br/>
    /// <see cref="Operand"/>s are what the command operates on.<br/>
    /// <see cref="Option"/>s are are how the command operates on the <see cref="Operand"/>s<br/>
    /// https://commanddotnet.bilal-fazlani.com/arguments/option-or-operand/
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class PositionalAttribute : OperandAttribute
    {
        /// <summary>
        /// Identifies a property or parameter as an <see cref="Operand"/>, aka positional argument.
        /// </summary>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure operands defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public PositionalAttribute([CallerLineNumber] int __callerLineNumber = 0) 
            : base(__callerLineNumber)
        {
        }

        /// <summary>
        /// Identifies a property or parameter as an <see cref="Operand"/>, aka positional argument.
        /// </summary>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure operands defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public PositionalAttribute(string name, [CallerLineNumber] int __callerLineNumber = 0)
            : base(name, __callerLineNumber)
        {
        }
    }
}