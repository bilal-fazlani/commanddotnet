using System;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Tokens;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class AppSettings : IIndentableToString
    {
        [Obsolete("Use CommandDefaults.BooleanMode instead")]
        public BooleanMode BooleanMode
        {
            get => CommandDefaults.BooleanMode;
            set => CommandDefaults.BooleanMode = value;
        }

        [Obsolete("Use CommandDefaults.IgnoreUnexpectedOperands instead")]
        public bool IgnoreUnexpectedOperands
        {
            get => CommandDefaults.IgnoreUnexpectedOperands;
            set => CommandDefaults.IgnoreUnexpectedOperands = value;
        }

        [Obsolete("Use CommandDefaults.ArgumentSeparatorStrategy instead")]
        public ArgumentSeparatorStrategy DefaultArgumentSeparatorStrategy
        {
            get => CommandDefaults.ArgumentSeparatorStrategy;
            set => CommandDefaults.ArgumentSeparatorStrategy = value;
        }

        [Obsolete("Use CommandDefaults.ArgumentMode instead")]
        public ArgumentMode DefaultArgumentMode
        {
            get => CommandDefaults.ArgumentMode;
            set => CommandDefaults.ArgumentMode = value;
        }

        public CommandSettings CommandDefaults { get; set; } = new CommandSettings();

        /// <summary>
        /// Set to true to prevent tokenizing arguments as <see cref="TokenType.Directive"/>,
        /// captured in <see cref="CommandContext.Tokens"/>.
        /// Arguments with the [directive syntax] will be tokenized
        /// as <see cref="TokenType.Value"/>.
        /// </summary>
        public bool DisableDirectives { get; set; }

        /// <summary>Settings specific to built-in help providers</summary>
        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        /// <summary>
        /// The collection of <see cref="IArgumentTypeDescriptor"/>'s use to convert arguments
        /// from the commandline to the parameter & property types for the command methods.
        /// </summary>
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return this.ToStringFromPublicProperties(indent);
        }
    }
}