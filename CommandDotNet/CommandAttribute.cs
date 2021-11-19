using System;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute, INameAndDescription
    {
        [Obsolete("Use constructor parameter instead")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Usage { get; set; }

        public string? ExtendedHelpText { get; set; }

        /// <summary>
        /// Overrides <see cref="AppSettings.IgnoreUnexpectedOperands"/><br/>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands
        {
            get => IgnoreUnexpectedOperandsAsNullable.GetValueOrDefault();
            set => IgnoreUnexpectedOperandsAsNullable = value;
        }

        internal bool? IgnoreUnexpectedOperandsAsNullable { get; private set; }

        /// <summary>
        /// The <see cref="ArgumentSeparatorStrategy"/> for the <see cref="Command"/>
        /// </summary>
        public ArgumentSeparatorStrategy ArgumentSeparatorStrategy
        {
            get => ArgumentSeparatorStrategyAsNullable.GetValueOrDefault();
            set => ArgumentSeparatorStrategyAsNullable = value;
        }

        internal ArgumentSeparatorStrategy? ArgumentSeparatorStrategyAsNullable { get; private set; }

        /// <summary>Indicates the class is a command. The name will be derived from the class name.</summary>
        public CommandAttribute()
        {
        }

        /// <summary>Indicates the class is a command with the given name</summary>
        public CommandAttribute(string? name)
        {
            Name = name;
        }
    }
}