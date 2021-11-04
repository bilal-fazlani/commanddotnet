﻿using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    /// <summary>
    /// <see cref="Operand"/>s are the positional arguments of a command.<br/>
    /// <see cref="Operand"/>s are what the command operates on.<br/>
    /// <see cref="Option"/>s are are how the command operates on the <see cref="Operand"/>s<br/>
    /// https://commanddotnet.bilal-fazlani.com/arguments/option-or-operand/
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class OperandAttribute : Attribute, INameAndDescription
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int CallerLineNumber { get; }

        /// <summary>
        /// Constructs an <see cref="OperandAttribute"/>
        /// </summary>
        /// <param name="__callerLineNumber">
        /// DO NOT USE. Populated by <see cref="CallerLineNumberAttribute"/>.<br/>
        /// This value is used to ensure operands defined in an <see cref="IArgumentModel"/>
        /// are positioned based on their property's order in the class definition.<br/>
        /// This value is ignored for parameters.
        /// </param>
        public OperandAttribute([CallerLineNumber] int __callerLineNumber = 0)
        {
            CallerLineNumber = __callerLineNumber;
        }
    }
}