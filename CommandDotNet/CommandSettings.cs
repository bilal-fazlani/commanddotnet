using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class CommandSettings : IIndentableToString, ICommandSettings
    {
        private BooleanMode _booleanMode = BooleanMode.Implicit;

        /// <summary>
        /// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
        /// When Implicit, boolean options are treated as Flags, considered false unless it's specified
        /// and the next argument will be considered a new argument.
        /// </summary>
        public BooleanMode BooleanMode
        {
            get => _booleanMode;
            set
            {
                if (value == BooleanMode.Unknown)
                    throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
                _booleanMode = value;
            }
        }

        /// <summary>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands { get; set; }

        /// <summary>
        /// The default <see cref="ArgumentSeparatorStrategy"/>.
        /// This can be overridden for a <see cref="Command"/> using the <see cref="CommandAttribute"/>
        /// </summary>
        public ArgumentSeparatorStrategy ArgumentSeparatorStrategy { get; set; } = ArgumentSeparatorStrategy.EndOfOptions;

        /// <summary>
        /// When arguments are not decorated with [Operand] or [Option]
        /// DefaultArgumentMode is used to determine which mode to use.
        /// Operand is the default.
        /// </summary>
        public ArgumentMode ArgumentMode { get; set; } = ArgumentMode.Operand;
        
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