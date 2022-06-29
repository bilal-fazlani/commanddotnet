using System;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute, INameAndDescription
    {
        internal bool? IgnoreUnexpectedOperandsAsNullable { get; private set; }
        internal ArgumentSeparatorStrategy? ArgumentSeparatorStrategyAsNullable { get; private set; }

        public string? Name { get; }

        // begin-snippet: CommandAttribute-properties
        /// <summary>The description to show in the help</summary>
        public string? Description { get; set; }

        /// <summary>The multiline description to show in the help</summary>
        public string[]? DescriptionLines { get; set; }

        /// <summary>The example of how to use this command</summary>
        public string? Usage { get; set; }

        /// <summary>The multiline example of how to use this command</summary>
        public string[]? UsageLines { get; set; }

        /// <summary>Additional help to show at the end of the help</summary>
        public string? ExtendedHelpText { get; set; }

        /// <summary>Additional multiline help to show at the end of the help</summary>
        public string[]? ExtendedHelpTextLines { get; set; }

        /// <summary>
        /// Overrides <see cref="ParseAppSettings.IgnoreUnexpectedOperands"/><br/>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands
        {
            get => IgnoreUnexpectedOperandsAsNullable.GetValueOrDefault();
            set => IgnoreUnexpectedOperandsAsNullable = value;
        }

        /// <summary>
        /// The <see cref="ArgumentSeparatorStrategy"/> for the <see cref="Command"/>
        /// </summary>
        public ArgumentSeparatorStrategy ArgumentSeparatorStrategy
        {
            get => ArgumentSeparatorStrategyAsNullable.GetValueOrDefault();
            set => ArgumentSeparatorStrategyAsNullable = value;
        }

        /// <summary>Indicates the class is a command. The name will be derived from the class name.</summary>
        public CommandAttribute()
        {
        }

        /// <summary>
        /// Indicates the class is a command with the given name.<br/>
        /// This is not required unless you plan to override a default property of the command.<br/>
        /// All public methods will be interpreted as commands.
        /// </summary>
        public CommandAttribute(string? name)
        {
            Name = name;
        }
        // end-snippet
    }
}