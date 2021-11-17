using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class ArgumentAppSettings :IIndentableToString
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