using System;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Tokens;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class AppSettings : IIndentableToString
    {
        [Obsolete("Use Arguments.BooleanMode")]
        public BooleanMode BooleanMode
        {
            get => Arguments.BooleanMode; 
            set => Arguments.BooleanMode = value;
        }

        [Obsolete("Use Parser.IgnoreUnexpectedOperands")]
        public bool IgnoreUnexpectedOperands
        {
            get => Parser.IgnoreUnexpectedOperands; 
            set => Parser.IgnoreUnexpectedOperands = value;
        }

        [Obsolete("Use Parser.IgnoreUnexpectedOperands")]
        public ArgumentSeparatorStrategy DefaultArgumentSeparatorStrategy
        {
            get => Parser.DefaultArgumentSeparatorStrategy;
            set => Parser.DefaultArgumentSeparatorStrategy = value;
        }

        [Obsolete("Use Arguments.DefaultArgumentMode")]
        public ArgumentMode DefaultArgumentMode
        {
            get => Arguments.DefaultArgumentMode;
            set => Arguments.DefaultArgumentMode = value;
        }

        /// <summary>
        /// Set to true to prevent tokenizing arguments as <see cref="TokenType.Directive"/>,
        /// captured in <see cref="CommandContext.Tokens"/>.
        /// Arguments with the [directive syntax] will be tokenized
        /// as <see cref="TokenType.Argument"/>.
        /// </summary>
        public bool DisableDirectives { get; set; }

        /// <summary>Settings specific to commands</summary>
        public CommandAppSettings Commands { get; set; } = new();

        /// <summary>Settings specific to arguments</summary>
        public ArgumentAppSettings Arguments { get; set; } = new();

        /// <summary>Settings specific to the command parsing</summary>
        public ParseAppSettings Parser { get; set; } = new();

        /// <summary>Settings specific to built-in help providers</summary>
        public AppHelpSettings Help { get; set; } = new();

        /// <summary>When specified, this function will be used to localize user output from the framework</summary>
        [Obsolete("Use Localization.Localize")]
        public Func<string, string?>? Localize
        {
            get => Localization.Localize; 
            set => Localization.Localize = value;
        }

        public LocalizationAppSettings Localization { get; set; } = new();
 
        /// <summary>
        /// The collection of <see cref="IArgumentTypeDescriptor"/>'s use to convert arguments
        /// from the commandline to the parameter & property types for the command methods.
        /// </summary>
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new();

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