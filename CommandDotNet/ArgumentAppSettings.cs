
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class ArgumentAppSettings : IIndentableToString
    {
        /// <summary>
        /// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
        /// When Implicit, boolean options are treated as Flags, considered false unless it's specified
        /// and the next argument will be considered a new argument.
        /// </summary>
        public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

        /// <summary>
        /// When arguments are not decorated with <see cref="OperandAttribute"/> or <see cref="OptionAttribute"/>
        /// DefaultArgumentMode is used to determine which type of argument to assign.
        /// <see cref="ArgumentMode.Operand"/> is the default.
        /// </summary>
        public ArgumentMode DefaultArgumentMode { get; set; } = ArgumentMode.Operand;

        /// <summary>
        /// Character used to split the option values into substrings.
        /// Setting it here will enable for all options that accept multiple values.<br/>
        /// The character can be set and overridden for each option in the <see cref="OptionAttribute"/>
        /// or <see cref="NamedAttribute"/>. 
        /// </summary>
        public char? DefaultOptionSplit { get; set; }

        /// <summary>
        /// Symbol used to indicate piped content should be directed to a specific argument.
        /// If not specified when executing a command, piped input will be directed to
        /// the final operand if it is a list.
        /// </summary>
        public string? DefaultPipeTargetSymbol { get; set; } = "^";

        /// <summary>
        /// When true, arity is not validated.
        /// Arity validation will also be skipped if the application does not support
        /// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references">NRTs</see>.
        /// </summary>
        public bool SkipArityValidation { get; set; }

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